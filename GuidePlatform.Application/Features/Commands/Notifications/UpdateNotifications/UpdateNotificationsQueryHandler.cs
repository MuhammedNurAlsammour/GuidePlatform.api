using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.Notifications.UpdateNotifications
{
  public class UpdateNotificationsCommandHandler : BaseCommandHandler, IRequestHandler<UpdateNotificationsCommandRequest, TransactionResultPack<UpdateNotificationsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateNotificationsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateNotificationsCommandResponse>> Handle(UpdateNotificationsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. notifications bul ve kontrol et - 1. notifications bul ve kontrol et
        var notifications = await _context.notifications
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (notifications == null)
        {
          return ResultFactory.CreateErrorResult<UpdateNotificationsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek notifications bulunamadı.",
            "notifications not found."
          );
        }

        // 🎯 2. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateNotificationsCommandRequest.Map(notifications, request, _currentUserService);

        // 🎯 3. Entity'yi context'e ekle - 3. Entity'yi context'e ekle
        _context.notifications.Update(notifications);

        // 🎯 4. Değişiklikleri kaydet - 4. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 5. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(notifications.Id, request, cancellationToken);

        // 🎯 6. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(notifications.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateNotificationsCommandResponse>(
          new UpdateNotificationsCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "notifications başarıyla güncellendi.",
          $"notifications Id: { notifications.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateNotificationsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "notifications güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid notificationsId, UpdateNotificationsCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid notificationsId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
