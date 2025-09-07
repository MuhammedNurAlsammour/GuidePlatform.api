using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Payments.DeletePayments
{
  public class DeletePaymentsCommandHandler : BaseCommandHandler, IRequestHandler<DeletePaymentsCommandRequest, TransactionResultPack<DeletePaymentsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeletePaymentsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeletePaymentsCommandResponse>> Handle(DeletePaymentsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var paymentss = await _context.payments
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (paymentss == null)
        {
          return ResultFactory.CreateErrorResult<DeletePaymentsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek payments bulunamadı.",
            "payments not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcesspaymentsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.payments.Remove(paymentss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeletePaymentsCommandResponse>(
          new DeletePaymentsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "payments silindi.",
          $"payments Id: payments.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeletePaymentsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "payments silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcesspaymentsAdditionalOperationsAsync(Guid paymentsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(paymentsId);
      // await _auditService.LogDeletionAsync(paymentsId, _currentUserService.UserId);
    }
  }
}