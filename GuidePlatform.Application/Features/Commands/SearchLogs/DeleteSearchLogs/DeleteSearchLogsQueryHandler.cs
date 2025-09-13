using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.SearchLogs.DeleteSearchLogs
{
  public class DeleteSearchLogsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteSearchLogsCommandRequest, TransactionResultPack<DeleteSearchLogsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteSearchLogsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteSearchLogsCommandResponse>> Handle(DeleteSearchLogsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var searchLogss = await _context.searchLogs
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (searchLogss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteSearchLogsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek searchLogs bulunamadı.",
            "searchLogs not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcesssearchLogsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.searchLogs.Remove(searchLogss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteSearchLogsCommandResponse>(
          new DeleteSearchLogsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "searchLogs silindi.",
          $"searchLogs Id: searchLogs.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteSearchLogsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "searchLogs silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcesssearchLogsAdditionalOperationsAsync(Guid searchLogsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(searchLogsId);
      // await _auditService.LogDeletionAsync(searchLogsId, _currentUserService.UserId);
    }
  }
}