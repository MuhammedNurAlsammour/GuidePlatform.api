using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.BusinessImages.UpdateBusinessImages
{
  public class UpdateBusinessImagesCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBusinessImagesCommandRequest, TransactionResultPack<UpdateBusinessImagesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateBusinessImagesCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
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

        // 🎯 4. Entity'yi context'e ekle - 4. Entity'yi context'e ekle
        _context.businessImages.Update(businessImages);

        // 🎯 5. Değişiklikleri kaydet - 5. Değişiklikleri kaydet
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
          $"businessImages Id: { businessImages.Id} başarıyla güncellendi."
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
  }
}
