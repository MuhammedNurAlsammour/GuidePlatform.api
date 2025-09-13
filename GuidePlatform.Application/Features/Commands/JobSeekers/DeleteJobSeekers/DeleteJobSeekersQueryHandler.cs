using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.DeleteJobSeekers
{
  public class DeleteJobSeekersCommandHandler : BaseCommandHandler, IRequestHandler<DeleteJobSeekersCommandRequest, TransactionResultPack<DeleteJobSeekersCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteJobSeekersCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteJobSeekersCommandResponse>> Handle(DeleteJobSeekersCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var jobSeekerss = await _context.jobSeekers
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (jobSeekerss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteJobSeekersCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek jobSeekers bulunamadı.",
            "jobSeekers not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessjobSeekersAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.jobSeekers.Remove(jobSeekerss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteJobSeekersCommandResponse>(
          new DeleteJobSeekersCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "jobSeekers silindi.",
          $"jobSeekers Id: jobSeekers.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteJobSeekersCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "jobSeekers silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessjobSeekersAdditionalOperationsAsync(Guid jobSeekersId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(jobSeekersId);
      // await _auditService.LogDeletionAsync(jobSeekersId, _currentUserService.UserId);
    }
  }
}