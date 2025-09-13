using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.BusinessImages.UpdateBusinessImages
{
  public class UpdateBusinessImagesCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBusinessImagesCommandRequest, TransactionResultPack<UpdateBusinessImagesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IBusinessImageService _businessImageService;

    public UpdateBusinessImagesCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IBusinessImageService businessImageService)
      : base(currentUserService)
    {
      _context = context;
      _businessImageService = businessImageService;
    }

    public async Task<TransactionResultPack<UpdateBusinessImagesCommandResponse>> Handle(UpdateBusinessImagesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. businessImages bul ve kontrol et - 1. businessImages bul ve kontrol et
        var businessImages = await _context.businessImages
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessImages == null)
        {
          return ResultFactory.CreateErrorResult<UpdateBusinessImagesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek businessImages bulunamadı.",
            "businessImages not found."
          );
        }

        // 🎯 3. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateBusinessImagesCommandRequest.Map(businessImages, request, _currentUserService);

        // 🎯 4. Yeni sistem: Eğer PhotoUrl veya ThumbnailUrl güncelleniyorsa, eski dosyaları sil
        if (!string.IsNullOrEmpty(request.PhotoUrl) || !string.IsNullOrEmpty(request.ThumbnailUrl))
        {
          // Eski dosyaları sil (opsiyonel - gerekirse)
          // await DeleteOldImageFiles(businessImages);
        }

        // 🎯 5. Entity'yi context'e ekle - 5. Entity'yi context'e ekle
        _context.businessImages.Update(businessImages);

        // 🎯 6. Değişiklikleri kaydet - 6. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 4. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(businessImages.Id, request, cancellationToken);

        // 🎯 5. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(businessImages.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateBusinessImagesCommandResponse>(
          new UpdateBusinessImagesCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "businessImages başarıyla güncellendi.",
          $"businessImages Id: {businessImages.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateBusinessImagesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessImages güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid businessImagesId, UpdateBusinessImagesCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid businessImagesId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }

    /// <summary>
    /// Eski resim dosyalarını siler - Deletes old image files
    /// </summary>
    // private async Task DeleteOldImageFiles(Domain.Entities.BusinessImagesViewModel businessImages)
    // {
    //   try
    //   {
    //     // Eski PhotoUrl dosyasını sil
    //     if (!string.IsNullOrEmpty(businessImages.PhotoUrl))
    //     {
    //       // FileStorageService kullanarak dosyayı sil
    //       // await _fileStorageService.DeleteImageAsync(businessImages.PhotoUrl);
    //     }
    //     
    //     // Eski ThumbnailUrl dosyasını sil
    //     if (!string.IsNullOrEmpty(businessImages.ThumbnailUrl))
    //     {
    //       // FileStorageService kullanarak dosyayı sil
    //       // await _fileStorageService.DeleteImageAsync(businessImages.ThumbnailUrl);
    //     }
    //   }
    //   catch (Exception ex)
    //   {
    //     // Log error but don't fail the update
    //     // _logger.LogWarning($"Failed to delete old image files: {ex.Message}");
    //   }
    // }
  }
}
