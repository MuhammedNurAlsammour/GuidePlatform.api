using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.NotificationSettings.DeleteNotificationSettings
{
  public class DeleteNotificationSettingsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteNotificationSettingsCommandRequest, TransactionResultPack<DeleteNotificationSettingsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteNotificationSettingsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteNotificationSettingsCommandResponse>> Handle(DeleteNotificationSettingsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var notificationSettingss = await _context.notificationSettings
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (notificationSettingss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteNotificationSettingsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek notificationSettings bulunamadı.",
            "notificationSettings not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessnotificationSettingsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.notificationSettings.Remove(notificationSettingss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteNotificationSettingsCommandResponse>(
          new DeleteNotificationSettingsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "notificationSettings silindi.",
          $"notificationSettings Id: notificationSettings.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteNotificationSettingsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "notificationSettings silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessnotificationSettingsAdditionalOperationsAsync(Guid notificationSettingsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(notificationSettingsId);
      // await _auditService.LogDeletionAsync(notificationSettingsId, _currentUserService.UserId);
    }
  }
}