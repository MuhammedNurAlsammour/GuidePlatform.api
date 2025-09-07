using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.NotificationSettings.UpdateNotificationSettings
{
  public class UpdateNotificationSettingsCommandHandler : BaseCommandHandler, IRequestHandler<UpdateNotificationSettingsCommandRequest, TransactionResultPack<UpdateNotificationSettingsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateNotificationSettingsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateNotificationSettingsCommandResponse>> Handle(UpdateNotificationSettingsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. notificationSettings bul ve kontrol et - 1. notificationSettings bul ve kontrol et
        var notificationSettings = await _context.notificationSettings
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (notificationSettings == null)
        {
          return ResultFactory.CreateErrorResult<UpdateNotificationSettingsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek notificationSettings bulunamadı.",
            "notificationSettings not found."
          );
        }

        // 🎯 2. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateNotificationSettingsCommandRequest.Map(notificationSettings, request, _currentUserService);

        // 🎯 3. Entity'yi context'e ekle - 3. Entity'yi context'e ekle
        _context.notificationSettings.Update(notificationSettings);

        // 🎯 4. Değişiklikleri kaydet - 4. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 5. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(notificationSettings.Id, request, cancellationToken);

        // 🎯 6. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(notificationSettings.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateNotificationSettingsCommandResponse>(
          new UpdateNotificationSettingsCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "notificationSettings başarıyla güncellendi.",
          $"notificationSettings Id: { notificationSettings.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateNotificationSettingsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "notificationSettings güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid notificationSettingsId, UpdateNotificationSettingsCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid notificationSettingsId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
