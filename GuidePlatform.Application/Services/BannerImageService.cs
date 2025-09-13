using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Domain.Entities;
using System.IO;
using Microsoft.Extensions.Logging;

namespace GuidePlatform.Application.Services
{
  public class BannerImageUploadDto : ImageUploadDto
  {
    public Guid BannerId { get; set; }
    public Guid? BannerImageId { get; set; } // Hangi kaydı güncelleyeceğimizi belirtmek için
  }

  public interface IBannerImageService
  {
    Task<TransactionResultPack<bool>> UpdateBannerImageAsync(BannerImageUploadDto bannerImageUploadDto);
    Task<TransactionResultPack<string>> GetBannerImageAsync(Guid bannerId);
    Task<TransactionResultPack<string>> GetBannerThumbnailAsync(Guid bannerId);
  }

  public class BannerImageService : IBannerImageService
  {
    private readonly IApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<BannerImageService> _logger;

    public BannerImageService(IApplicationDbContext context, IImageService imageService, IFileStorageService fileStorageService, ILogger<BannerImageService> logger)
    {
      _context = context;
      _imageService = imageService;
      _fileStorageService = fileStorageService;
      _logger = logger;
    }

    public async Task<TransactionResultPack<bool>> UpdateBannerImageAsync(BannerImageUploadDto bannerImageUploadDto)
    {
      try
      {
        // التحقق من وجود بيانات الصورة - إذا لم توجد، نعتبر العملية ناجحة بدون معالجة
        if (string.IsNullOrEmpty(bannerImageUploadDto.PhotoPath) && string.IsNullOrEmpty(bannerImageUploadDto.PhotoBase64))
        {
          return ResultFactory.CreateSuccessResult<bool>(
              true,
              bannerImageUploadDto.BannerId,
              null,
              "İşlem Başarılı",
              "Banner oluşturuldu (fotoğraf olmadan).",
              $"Banner ID: {bannerImageUploadDto.BannerId} fotoğraf olmadan oluşturuldu."
          );
        }

        // إعطاء الأولوية لـ PhotoBase64 إذا كان كلاهما موجودين
        if (!string.IsNullOrEmpty(bannerImageUploadDto.PhotoBase64))
        {
          bannerImageUploadDto.PhotoPath = null;
        }
        else if (!string.IsNullOrEmpty(bannerImageUploadDto.PhotoPath))
        {
          bannerImageUploadDto.PhotoBase64 = null;
        }

        // التحقق من صحة BannerId
        if (bannerImageUploadDto.BannerId == Guid.Empty)
        {
          return ResultFactory.CreateErrorResult<bool>(
              bannerImageUploadDto.BannerId,
              null,
              "Hata / Geçersiz Banner ID",
              "Geçersiz banner ID'si.",
              "BannerId geçerli bir GUID olmalıdır."
          );
        }

        // التحقق من وجود Banner
        var banner = await _context.banners
            .FirstOrDefaultAsync(b => b.Id == bannerImageUploadDto.BannerId);

        if (banner == null)
        {
          return ResultFactory.CreateErrorResult<bool>(
              bannerImageUploadDto.BannerId,
              null,
              "Hata / Banner Bulunamadı",
              "Belirtilen banner bulunamadı.",
              "Banner veritabanında bulunamadı."
          );
        }

        var (photoData, thumbnailData) = await _imageService.ProcessImageAsync(bannerImageUploadDto);
        await SaveImageToFileSystemAsync(photoData, thumbnailData, bannerImageUploadDto);

        return ResultFactory.CreateSuccessResult<bool>(
            true,
            bannerImageUploadDto.BannerId,
            null,
            "İşlem Başarılı",
            "Banner fotoğrafı başarıyla güncellendi.",
            $"Banner ID: {bannerImageUploadDto.BannerId} fotoğrafı kaydedildi."
        );
      }
      catch (ArgumentException ex)
      {
        _logger.LogError(ex, "Banner image validation error for BannerId: {BannerId}, PhotoPath: {PhotoPath}",
            bannerImageUploadDto.BannerId, bannerImageUploadDto.PhotoPath);
        return ResultFactory.CreateErrorResult<bool>(
            bannerImageUploadDto.BannerId,
            null,
            "Hata / Fotoğraf Doğrulama",
            "Fotoğraf verileri geçersiz.",
            $"Detay: {ex.Message} | Inner: {ex.InnerException?.Message}"
        );
      }
      catch (FileNotFoundException ex)
      {
        _logger.LogError(ex, "Banner image file not found for BannerId: {BannerId}, PhotoPath: {PhotoPath}",
            bannerImageUploadDto.BannerId, bannerImageUploadDto.PhotoPath);
        return ResultFactory.CreateErrorResult<bool>(
            bannerImageUploadDto.BannerId,
            null,
            "Hata / Dosya Bulunamadı",
            "Fotoğraf dosyası bulunamadı.",
            $"Detay: {ex.Message} | Inner: {ex.InnerException?.Message}"
        );
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Banner image processing error for BannerId: {BannerId}, PhotoPath: {PhotoPath}",
            bannerImageUploadDto.BannerId, bannerImageUploadDto.PhotoPath);
        return ResultFactory.CreateErrorResult<bool>(
            bannerImageUploadDto.BannerId,
            null,
            "Hata / Fotoğraf İşleme",
            "Fotoğraf işlenirken hata oluştu.",
            $"Detay: {ex.Message} | Inner: {ex.InnerException?.Message} | StackTrace: {ex.StackTrace}"
        );
      }
    }

    public async Task<TransactionResultPack<string>> GetBannerImageAsync(Guid bannerId)
    {
      try
      {
        var banner = await _context.banners
            .FirstOrDefaultAsync(b => b.Id == bannerId);

        if (banner?.Photo == null)
        {
          return ResultFactory.CreateErrorResult<string>(
              bannerId,
              null,
              "Hata / Fotoğraf Bulunamadı",
              "Belirtilen banner'a ait fotoğraf bulunamadı.",
              "Banner image not found."
          );
        }

        string photoDataUrl = _imageService.GetBase64Image(banner.Photo, banner.PhotoContentType ?? "image/jpeg");

        return ResultFactory.CreateSuccessResult<string>(
            photoDataUrl,
            bannerId,
            null,
            "İşlem Başarılı",
            "Banner fotoğrafı başarıyla getirildi.",
            "Banner image retrieved successfully."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<string>(
            bannerId,
            null,
            "Hata / Fotoğraf Getirme",
            "Fotoğraf getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    public async Task<TransactionResultPack<string>> GetBannerThumbnailAsync(Guid bannerId)
    {
      try
      {
        var banner = await _context.banners
            .FirstOrDefaultAsync(b => b.Id == bannerId);

        if (banner?.Thumbnail == null)
        {
          return ResultFactory.CreateErrorResult<string>(
              bannerId,
              null,
              "Hata / Küçük Resim Bulunamadı",
              "Belirtilen banner'a ait küçük resim bulunamadı.",
              "Banner thumbnail not found."
          );
        }

        string photoDataUrl = _imageService.GetBase64Image(banner.Thumbnail);

        return ResultFactory.CreateSuccessResult<string>(
            photoDataUrl,
            bannerId,
            null,
            "İşlem Başarılı",
            "Banner küçük resmi başarıyla getirildi.",
            "Banner thumbnail retrieved successfully."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<string>(
            bannerId,
            null,
            "Hata / Küçük Resim Getirme",
            "Küçük resim getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    private async Task SaveImageToFileSystemAsync(byte[] photoData, byte[] thumbnailData, BannerImageUploadDto bannerImageUploadDto)
    {
      try
      {
        // التحقق من حجم البيانات
        if (photoData.Length > 10 * 1024 * 1024) // 10MB
        {
          throw new ArgumentException("Fotoğraf çok büyük! Maksimum 10MB olabilir.");
        }

        if (thumbnailData.Length > 1024 * 1024) // 1MB
        {
          throw new ArgumentException("Küçük resim çok büyük! Maksimum 1MB olabilir.");
        }

        // Dosya adlarını oluştur - Create file names
        var photoFileName = $"banner_{bannerImageUploadDto.BannerId}_photo_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";
        var thumbnailFileName = $"banner_{bannerImageUploadDto.BannerId}_thumb_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";

        // wwwroot'a kaydet - Save to wwwroot
        var photoResult = await _fileStorageService.SaveImageAsync(photoData, photoFileName, "images/banners");
        var thumbnailResult = await _fileStorageService.SaveImageAsync(thumbnailData, thumbnailFileName, "images/banners");

        if (!photoResult.OperationStatus || !thumbnailResult.OperationStatus)
        {
          throw new Exception($"Resimler wwwroot'a kaydedilemedi. Photo: {photoResult.OperationResult.MessageContent}, Thumbnail: {thumbnailResult.OperationResult.MessageContent}");
        }

        // Mevcut banner kaydını bul ve güncelle - Find and update existing banner record
        var existingBanner = await _context.banners
            .FirstOrDefaultAsync(b => b.Id == bannerImageUploadDto.BannerId);

        if (existingBanner != null)
        {
          // Mevcut kaydı güncelle - Update existing record
          existingBanner.PhotoUrl = photoResult.Result;
          existingBanner.ThumbnailUrl = thumbnailResult.Result;
          existingBanner.PhotoContentType = "image/jpeg";
          existingBanner.RowUpdatedDate = DateTime.UtcNow;
        }
        else
        {
          throw new Exception("Banner kaydı bulunamadı!");
        }

        var result = await _context.SaveChangesAsync();

        if (result == 0)
        {
          throw new Exception("Fotoğraf veritabanına kaydedilemedi!");
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Fotoğraf wwwroot'a kaydedilirken hata oluştu: {ex.Message}", ex);
      }
    }
  }
}
