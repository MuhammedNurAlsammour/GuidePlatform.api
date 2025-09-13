using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Application.Services;

namespace GuidePlatform.Application.Features.Commands.Banners.CreateBanners
{
  public class CreateBannersCommandHandler : BaseCommandHandler, IRequestHandler<CreateBannersCommandRequest, TransactionResultPack<CreateBannersCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly IBannerImageService _bannerImageService;

    public CreateBannersCommandHandler(
        IApplicationDbContext context,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IBannerImageService bannerImageService
    ) : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
      _bannerImageService = bannerImageService;
    }



    public async Task<TransactionResultPack<CreateBannersCommandResponse>> Handle(CreateBannersCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // Kullanıcı bilgilerini kontrol et
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // التحقق من وجود بانر مطابق تماماً قبل الإنشاء
        // تحضير القيم للمقارنة لتجنب null في lambda
        var requestTitleLower = request.Title?.Trim().ToLower() ?? "";
        var requestDescriptionLower = request.Description?.Trim().ToLower() ?? "";

        var existingBanner = await _context.banners
            .FirstOrDefaultAsync(b =>
                (b.Title ?? "").Trim().ToLower() == requestTitleLower &&
                (b.Description ?? "").Trim().ToLower() == requestDescriptionLower &&
                b.AuthUserId == request.AuthUserId &&
                b.AuthCustomerId == request.AuthCustomerId &&
                b.ProvinceId == request.ProvinceId &&
                b.RowIsActive == true &&
                b.RowIsDeleted == false, cancellationToken);

        if (existingBanner != null)
        {
          bool hasLocalFilePath = !string.IsNullOrEmpty(request.PhotoUrl) &&
                                  (request.PhotoUrl.StartsWith("C:\\") || request.PhotoUrl.StartsWith("D:\\") ||
                                   request.PhotoUrl.StartsWith("E:\\"));

          if (hasLocalFilePath)
          {
            return ResultFactory.CreateErrorResult<CreateBannersCommandResponse>(
                existingBanner?.Id.ToString(),
                null,
                "Hata / Dosya Yolu",
                "Üretim ortamında yerel dosya yolları desteklenmez.",
                "Lütfen fotoğrafı Base64 formatında gönderin veya geçerli bir HTTP URL kullanın. Yerel dosya yolları (C:\\, D:\\) üretim ortamında çalışmaz."
            );
          }

          // فحص وجود صورة للمعالجة: Base64, PhotoPath, أو URL صحيح
          bool hasValidUrl = !string.IsNullOrEmpty(request.PhotoUrl) &&
                            (request.PhotoUrl.StartsWith("http://") || request.PhotoUrl.StartsWith("https://"));
          bool hasImageToProcess = !string.IsNullOrEmpty(request.PhotoBase64) ||
                                  !string.IsNullOrEmpty(request.PhotoPath) ||
                                  hasValidUrl;

          if (hasImageToProcess)
          {
            // نحدث صورة البانر الموجود
            var photoUploadDto = request.ToPhotoUploadDto(existingBanner.Id);
            var photoResult = await _bannerImageService.UpdateBannerImageAsync(photoUploadDto);

            if (!photoResult.OperationStatus)
            {
              return ResultFactory.CreateErrorResult<CreateBannersCommandResponse>(
                  existingBanner.Id.ToString(),
                  null,
                  photoResult.OperationResult.MessageTitle ?? "Hata / Fotoğraf Yükleme",
                  photoResult.OperationResult.MessageContent ?? "Fotoğraf yüklenirken bir hata oluştu.",
                  photoResult.OperationResult.MessageDetail ?? "Detay bilgisi bulunamadı."
              );
            }

            // إرجاع البانر المحدث
            return ResultFactory.CreateSuccessResult<CreateBannersCommandResponse>(
                new CreateBannersCommandResponse
                {
                  Id = existingBanner.Id,
                  StatusCode = 200, // Updated instead of created
                  Message = "Banner fotoğrafı güncellendi",
                  Timestamp = DateTime.UtcNow
                },
                existingBanner.Id.ToString(),
                null,
                "İşlem Başarılı",
                "Mevcut banner fotoğrafı başarıyla güncellendi.",
                $"Banner ID: {existingBanner.Id} fotoğrafı güncellendi."
            );
          }
          else
          {
            // إذا لم تكن هناك صورة للمعالجة، فهو تكرار حقيقي
            return ResultFactory.CreateErrorResult<CreateBannersCommandResponse>(
                existingBanner.Id.ToString(),
                null,
                "Hata / Tekrar Eden Banner",
                "Aynı başlık ve açıklamada bir banner zaten mevcut.",
                $"Banner başlığı '{request.Title}' ve açıklaması '{request.Description}' zaten kullanılıyor. Mevcut Banner ID: {existingBanner.Id}. Lütfen farklı bir başlık veya açıklama kullanın."
            );
          }
        }

        // فحص مسارات الملفات المحلية قبل إنشاء البانر الجديد
        bool hasLocalFilePathForNew = !string.IsNullOrEmpty(request.PhotoUrl) &&
                                      (request.PhotoUrl.StartsWith("C:\\") || request.PhotoUrl.StartsWith("D:\\") ||
                                       request.PhotoUrl.StartsWith("E:\\"));

        if (hasLocalFilePathForNew)
        {
          return ResultFactory.CreateErrorResult<CreateBannersCommandResponse>(
              null,
              null,
              "Hata / Dosya Yolu",
              "Üretim ortamında yerel dosya yolları desteklenmez.",
              "Lütfen fotoğrafı Base64 formatında gönderin veya geçerli bir HTTP URL kullanın. Yerel dosya yolları (C:\\, D:\\) üretim ortamında çalışmaz."
          );
        }

        // 1. banners oluştur (önce URL'ler olmadan)
        var banners = CreateBannersCommandRequest.Map(request, _currentUserService);
        await _context.banners.AddAsync(banners, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Fotoğraf yükleme işlemi - Base64, PhotoPath, أو URL صحيح
        bool hasValidUrlForNew = !string.IsNullOrEmpty(request.PhotoUrl) &&
                                (request.PhotoUrl.StartsWith("http://") || request.PhotoUrl.StartsWith("https://"));
        if (!string.IsNullOrEmpty(request.PhotoPath) || !string.IsNullOrEmpty(request.PhotoBase64) || hasValidUrlForNew)
        {
          var photoUploadDto = request.ToPhotoUploadDto(banners.Id);
          var photoResult = await _bannerImageService.UpdateBannerImageAsync(photoUploadDto);

          // photoResult.OperationStatus özelliği ile kontrol ediliyor
          if (!photoResult.OperationStatus)
          {
            return ResultFactory.CreateErrorResult<CreateBannersCommandResponse>(
                banners.Id.ToString(),
                null,
                photoResult.OperationResult.MessageTitle ?? "Hata / Fotoğraf Yükleme",
                photoResult.OperationResult.MessageContent ?? "Fotoğraf yüklenirken bir hata oluştu.",
                photoResult.OperationResult.MessageDetail ?? "Detay bilgisi bulunamadı."
            );
          }
        }
        // Yeni sistem: PhotoUrl ve ThumbnailUrl zaten Map method'unda kaydedildi (eğer URL ise)

        return ResultFactory.CreateSuccessResult<CreateBannersCommandResponse>(
            new CreateBannersCommandResponse
            {
              StatusCode = (int)HttpStatusCode.Created,
              Id = banners.Id
            },
            null,
            null,
            "İşlem Başarılı",
            "banners başarıyla oluşturuldu.",
            $"banners Id: {banners.Id} başarıyla oluşturuldu."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<CreateBannersCommandResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "banners oluşturulurken bir hata oluştu.",
            ex.InnerException?.Message ?? ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid bannersId, CreateBannersCommandRequest request, CancellationToken cancellationToken)
    // {
    //     // Ek işlemler buraya eklenebilir
    //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
    // }
  }
}
