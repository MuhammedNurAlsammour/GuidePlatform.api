using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessAnalytics.DeleteBusinessAnalytics
{
  public class DeleteBusinessAnalyticsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBusinessAnalyticsCommandRequest, TransactionResultPack<DeleteBusinessAnalyticsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteBusinessAnalyticsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteBusinessAnalyticsCommandResponse>> Handle(DeleteBusinessAnalyticsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var businessAnalyticss = await _context.businessAnalytics
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessAnalyticss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteBusinessAnalyticsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek businessAnalytics bulunamadı.",
            "businessAnalytics not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessbusinessAnalyticsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.businessAnalytics.Remove(businessAnalyticss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteBusinessAnalyticsCommandResponse>(
          new DeleteBusinessAnalyticsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "businessAnalytics silindi.",
          $"businessAnalytics Id: businessAnalytics.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteBusinessAnalyticsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessAnalytics silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessbusinessAnalyticsAdditionalOperationsAsync(Guid businessAnalyticsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(businessAnalyticsId);
      // await _auditService.LogDeletionAsync(businessAnalyticsId, _currentUserService.UserId);
    }
  }
}