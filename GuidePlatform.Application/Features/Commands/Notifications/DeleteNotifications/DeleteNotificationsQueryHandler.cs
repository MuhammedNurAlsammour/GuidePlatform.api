using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Notifications.DeleteNotifications
{
  public class DeleteNotificationsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteNotificationsCommandRequest, TransactionResultPack<DeleteNotificationsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteNotificationsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteNotificationsCommandResponse>> Handle(DeleteNotificationsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var notificationss = await _context.notifications
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (notificationss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteNotificationsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek notifications bulunamadı.",
            "notifications not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessnotificationsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.notifications.Remove(notificationss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteNotificationsCommandResponse>(
          new DeleteNotificationsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "notifications silindi.",
          $"notifications Id: notifications.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteNotificationsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "notifications silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessnotificationsAdditionalOperationsAsync(Guid notificationsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(notificationsId);
      // await _auditService.LogDeletionAsync(notificationsId, _currentUserService.UserId);
    }
  }
}