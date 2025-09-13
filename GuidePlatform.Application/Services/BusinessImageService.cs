using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Services
{
  public class BusinessImageUploadDto : ImageUploadDto
  {
    public Guid BusinessId { get; set; }
    public Guid? BusinessImageId { get; set; } // Hangi kaydı güncelleyeceğimizi belirtmek için
  }

  public interface IBusinessImageService
  {
    Task<TransactionResultPack<bool>> UpdateBusinessImageAsync(BusinessImageUploadDto businessImageUploadDto);
    Task<TransactionResultPack<string>> GetBusinessImageAsync(Guid businessId);
    Task<TransactionResultPack<string>> GetBusinessThumbnailAsync(Guid businessId);
  }

  public class BusinessImageService : IBusinessImageService
  {
    private readonly IApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly IFileStorageService _fileStorageService;

    public BusinessImageService(IApplicationDbContext context, IImageService imageService, IFileStorageService fileStorageService)
    {
      _context = context;
      _imageService = imageService;
      _fileStorageService = fileStorageService;
    }

    public async Task<TransactionResultPack<bool>> UpdateBusinessImageAsync(BusinessImageUploadDto businessImageUploadDto)
    {
      try
      {
        // التحقق من وجود بيانات الصورة
        if (string.IsNullOrEmpty(businessImageUploadDto.PhotoPath) && string.IsNullOrEmpty(businessImageUploadDto.PhotoBase64))
        {
          return ResultFactory.CreateErrorResult<bool>(
              businessImageUploadDto.BusinessId,
              null,
              "Hata / Fotoğraf Verisi",
              "Fotoğraf verisi bulunamadı.",
              "PhotoPath ve PhotoBase64 alanlarından en az biri doldurulmalıdır."
          );
        }

        // إعطاء الأولوية لـ PhotoBase64 إذا كان كلاهما موجودين
        if (!string.IsNullOrEmpty(businessImageUploadDto.PhotoBase64))
        {
          businessImageUploadDto.PhotoPath = null;
        }
        else if (!string.IsNullOrEmpty(businessImageUploadDto.PhotoPath))
        {
          businessImageUploadDto.PhotoBase64 = null;
        }

        // التحقق من صحة BusinessId
        if (businessImageUploadDto.BusinessId == Guid.Empty)
        {
          return ResultFactory.CreateErrorResult<bool>(
              businessImageUploadDto.BusinessId,
              null,
              "Hata / Geçersiz İşletme ID",
              "Geçersiz işletme ID'si.",
              "BusinessId geçerli bir GUID olmalıdır."
          );
        }

        // التحقق من وجود الإدارة
        var business = await _context.businesses
            .FirstOrDefaultAsync(b => b.Id == businessImageUploadDto.BusinessId);

        if (business == null)
        {
          return ResultFactory.CreateErrorResult<bool>(
              businessImageUploadDto.BusinessId,
              null,
              "Hata / İşletme Bulunamadı",
              "Belirtilen işletme bulunamadı.",
              "İşletme veritabanında bulunamadı."
          );
        }

        var (photoData, thumbnailData) = await _imageService.ProcessImageAsync(businessImageUploadDto);
        await SaveImageToFileSystemAsync(photoData, thumbnailData, businessImageUploadDto);

        return ResultFactory.CreateSuccessResult<bool>(
            true,
            businessImageUploadDto.BusinessId,
            null,
            "İşlem Başarılı",
            "İşletme fotoğrafı başarıyla güncellendi.",
            $"İşletme ID: {businessImageUploadDto.BusinessId} fotoğrafı kaydedildi."
        );
      }
      catch (ArgumentException ex)
      {
        return ResultFactory.CreateErrorResult<bool>(
            businessImageUploadDto.BusinessId,
            null,
            "Hata / Fotoğraf Doğrulama",
            "Fotoğraf verileri geçersiz.",
            ex.Message
        );
      }
      catch (FileNotFoundException ex)
      {
        return ResultFactory.CreateErrorResult<bool>(
            businessImageUploadDto.BusinessId,
            null,
            "Hata / Fotoğraf Bulunamadı",
            "Fotoğraf dosyası bulunamadı.",
            ex.Message
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<bool>(
            businessImageUploadDto.BusinessId,
            null,
            "Hata / Fotoğraf Yükleme",
            "Fotoğraf yüklenirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    public async Task<TransactionResultPack<string>> GetBusinessImageAsync(Guid businessId)
    {
      try
      {
        var businessImage = await _context.businessImages
            .FirstOrDefaultAsync(bi => bi.BusinessId == businessId && bi.IsPrimary);

        if (businessImage?.Photo == null)
        {
          return ResultFactory.CreateErrorResult<string>(
              businessId,
              null,
              "Hata / Fotoğraf Bulunamadı",
              "Belirtilen işletmeye ait fotoğraf bulunamadı.",
              "Business image not found."
          );
        }

        string photoDataUrl = _imageService.GetBase64Image(businessImage.Photo, businessImage.PhotoContentType ?? "image/jpeg");

        return ResultFactory.CreateSuccessResult<string>(
            photoDataUrl,
            businessId,
            null,
            "İşlem Başarılı",
            "İşletme fotoğrafı başarıyla getirildi.",
            "Business image retrieved successfully."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<string>(
            businessId,
            null,
            "Hata / Fotoğraf Getirme",
            "Fotoğraf getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    public async Task<TransactionResultPack<string>> GetBusinessThumbnailAsync(Guid businessId)
    {
      try
      {
        var businessImage = await _context.businessImages
            .FirstOrDefaultAsync(bi => bi.BusinessId == businessId && bi.IsPrimary);

        if (businessImage?.Thumbnail == null)
        {
          return ResultFactory.CreateErrorResult<string>(
              businessId,
              null,
              "Hata / Küçük Resim Bulunamadı",
              "Belirtilen işletmeye ait küçük resim bulunamadı.",
              "Business thumbnail not found."
          );
        }

        string photoDataUrl = _imageService.GetBase64Image(businessImage.Thumbnail);

        return ResultFactory.CreateSuccessResult<string>(
            photoDataUrl,
            businessId,
            null,
            "İşlem Başarılı",
            "İşletme küçük resmi başarıyla getirildi.",
            "Business thumbnail retrieved successfully."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<string>(
            businessId,
            null,
            "Hata / Küçük Resim Getirme",
            "Küçük resim getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    private async Task SaveImageToFileSystemAsync(byte[] photoData, byte[] thumbnailData, BusinessImageUploadDto businessImageUploadDto)
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
        var photoFileName = $"business_{businessImageUploadDto.BusinessId}_photo_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";
        var thumbnailFileName = $"business_{businessImageUploadDto.BusinessId}_thumb_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";

        // wwwroot'a kaydet - Save to wwwroot
        var photoResult = await _fileStorageService.SaveImageAsync(photoData, photoFileName, "images/businesses");
        var thumbnailResult = await _fileStorageService.SaveImageAsync(thumbnailData, thumbnailFileName, "images/businesses");

        if (!photoResult.OperationStatus || !thumbnailResult.OperationStatus)
        {
          throw new Exception($"Resimler wwwroot'a kaydedilemedi. Photo: {photoResult.OperationResult.MessageContent}, Thumbnail: {thumbnailResult.OperationResult.MessageContent}");
        }

        // Mevcut business image kaydını bul ve güncelle - Find and update existing business image record
        BusinessImagesViewModel? existingBusinessImage = null;

        if (businessImageUploadDto.BusinessImageId.HasValue)
        {
          // Belirli bir kaydı güncelle - Update specific record
          existingBusinessImage = await _context.businessImages
              .FirstOrDefaultAsync(bi => bi.Id == businessImageUploadDto.BusinessImageId.Value);
        }
        else
        {
          // En son oluşturulan kaydı bul (aynı business_id için) - Find the most recently created record for the same business_id
          existingBusinessImage = await _context.businessImages
              .Where(bi => bi.BusinessId == businessImageUploadDto.BusinessId)
              .OrderByDescending(bi => bi.RowCreatedDate)
              .FirstOrDefaultAsync();
        }

        if (existingBusinessImage != null)
        {
          // Mevcut kaydı güncelle - Update existing record
          existingBusinessImage.PhotoUrl = photoResult.Result;
          existingBusinessImage.ThumbnailUrl = thumbnailResult.Result;
          existingBusinessImage.PhotoContentType = "image/jpeg";
          existingBusinessImage.RowUpdatedDate = DateTime.UtcNow;
        }
        else
        {
          // Bu durumda yeni kayıt oluşturmaya gerek yok çünkü CreateBusinessImagesQueryHandler zaten oluşturdu
          // Bu durum sadece BusinessImageService doğrudan çağrıldığında olur
          throw new Exception("Business image kaydı bulunamadı! Önce CreateBusinessImages API'sini kullanın.");
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
