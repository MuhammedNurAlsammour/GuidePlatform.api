using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.DeleteJobOpportunities
{
  public class DeleteJobOpportunitiesCommandHandler : BaseCommandHandler, IRequestHandler<DeleteJobOpportunitiesCommandRequest, TransactionResultPack<DeleteJobOpportunitiesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteJobOpportunitiesCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteJobOpportunitiesCommandResponse>> Handle(DeleteJobOpportunitiesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var jobOpportunitiess = await _context.jobOpportunities
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (jobOpportunitiess == null)
        {
          return ResultFactory.CreateErrorResult<DeleteJobOpportunitiesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek jobOpportunities bulunamadı.",
            "jobOpportunities not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessjobOpportunitiesAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.jobOpportunities.Remove(jobOpportunitiess);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteJobOpportunitiesCommandResponse>(
          new DeleteJobOpportunitiesCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "jobOpportunities silindi.",
          $"jobOpportunities Id: jobOpportunities.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteJobOpportunitiesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "jobOpportunities silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessjobOpportunitiesAdditionalOperationsAsync(Guid jobOpportunitiesId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(jobOpportunitiesId);
      // await _auditService.LogDeletionAsync(jobOpportunitiesId, _currentUserService.UserId);
    }
  }
}