using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.UserVisits.DeleteUserVisits
{
  public class DeleteUserVisitsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteUserVisitsCommandRequest, TransactionResultPack<DeleteUserVisitsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteUserVisitsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteUserVisitsCommandResponse>> Handle(DeleteUserVisitsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var userVisitss = await _context.userVisits
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (userVisitss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteUserVisitsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek userVisits bulunamadı.",
            "userVisits not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessuserVisitsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.userVisits.Remove(userVisitss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteUserVisitsCommandResponse>(
          new DeleteUserVisitsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "userVisits silindi.",
          $"userVisits Id: userVisits.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteUserVisitsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "userVisits silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessuserVisitsAdditionalOperationsAsync(Guid userVisitsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(userVisitsId);
      // await _auditService.LogDeletionAsync(userVisitsId, _currentUserService.UserId);
    }
  }
}