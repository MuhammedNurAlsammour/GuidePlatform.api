using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Subscriptions.DeleteSubscriptions
{
  public class DeleteSubscriptionsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteSubscriptionsCommandRequest, TransactionResultPack<DeleteSubscriptionsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteSubscriptionsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteSubscriptionsCommandResponse>> Handle(DeleteSubscriptionsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var subscriptionss = await _context.subscriptions
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (subscriptionss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteSubscriptionsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek subscriptions bulunamadı.",
            "subscriptions not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcesssubscriptionsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.subscriptions.Remove(subscriptionss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteSubscriptionsCommandResponse>(
          new DeleteSubscriptionsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "subscriptions silindi.",
          $"subscriptions Id: subscriptions.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteSubscriptionsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "subscriptions silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcesssubscriptionsAdditionalOperationsAsync(Guid subscriptionsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(subscriptionsId);
      // await _auditService.LogDeletionAsync(subscriptionsId, _currentUserService.UserId);
    }
  }
}