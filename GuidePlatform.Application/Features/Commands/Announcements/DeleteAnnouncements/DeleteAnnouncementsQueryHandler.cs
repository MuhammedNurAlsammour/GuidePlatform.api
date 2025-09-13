using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Announcements.DeleteAnnouncements
{
  public class DeleteAnnouncementsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteAnnouncementsCommandRequest, TransactionResultPack<DeleteAnnouncementsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteAnnouncementsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteAnnouncementsCommandResponse>> Handle(DeleteAnnouncementsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var announcementss = await _context.announcements
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (announcementss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteAnnouncementsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek announcements bulunamadı.",
            "announcements not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessannouncementsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.announcements.Remove(announcementss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteAnnouncementsCommandResponse>(
          new DeleteAnnouncementsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "announcements silindi.",
          $"announcements Id: announcements.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteAnnouncementsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "announcements silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessannouncementsAdditionalOperationsAsync(Guid announcementsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(announcementsId);
      // await _auditService.LogDeletionAsync(announcementsId, _currentUserService.UserId);
    }
  }
}