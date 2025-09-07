using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Pages.DeletePages
{
  public class DeletePagesCommandHandler : BaseCommandHandler, IRequestHandler<DeletePagesCommandRequest, TransactionResultPack<DeletePagesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeletePagesCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeletePagesCommandResponse>> Handle(DeletePagesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var pagess = await _context.pages
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (pagess == null)
        {
          return ResultFactory.CreateErrorResult<DeletePagesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek pages bulunamadı.",
            "pages not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcesspagesAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.pages.Remove(pagess);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeletePagesCommandResponse>(
          new DeletePagesCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "pages silindi.",
          $"pages Id: pages.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeletePagesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "pages silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcesspagesAdditionalOperationsAsync(Guid pagesId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(pagesId);
      // await _auditService.LogDeletionAsync(pagesId, _currentUserService.UserId);
    }
  }
}