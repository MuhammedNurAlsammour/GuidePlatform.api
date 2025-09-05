using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessReviews.DeleteBusinessReviews
{
  public class DeleteBusinessReviewsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBusinessReviewsCommandRequest, TransactionResultPack<DeleteBusinessReviewsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteBusinessReviewsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteBusinessReviewsCommandResponse>> Handle(DeleteBusinessReviewsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var businessReviewss = await _context.businessReviews
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessReviewss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteBusinessReviewsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek businessReviews bulunamadı.",
            "businessReviews not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessbusinessReviewsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.businessReviews.Remove(businessReviewss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteBusinessReviewsCommandResponse>(
          new DeleteBusinessReviewsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "businessReviews silindi.",
          $"businessReviews Id: businessReviews.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteBusinessReviewsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessReviews silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessbusinessReviewsAdditionalOperationsAsync(Guid businessReviewsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(businessReviewsId);
      // await _auditService.LogDeletionAsync(businessReviewsId, _currentUserService.UserId);
    }
  }
}