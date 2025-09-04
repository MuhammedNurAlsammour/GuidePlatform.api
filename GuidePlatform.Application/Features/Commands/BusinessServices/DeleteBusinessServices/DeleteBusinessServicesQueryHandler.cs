using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessServices.DeleteBusinessServices
{
  public class DeleteBusinessServicesCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBusinessServicesCommandRequest, TransactionResultPack<DeleteBusinessServicesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteBusinessServicesCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteBusinessServicesCommandResponse>> Handle(DeleteBusinessServicesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var businessServicess = await _context.businessServices
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessServicess == null)
        {
          return ResultFactory.CreateErrorResult<DeleteBusinessServicesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek businessServices bulunamadı.",
            "businessServices not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessbusinessServicesAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.businessServices.Remove(businessServicess);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteBusinessServicesCommandResponse>(
          new DeleteBusinessServicesCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "businessServices silindi.",
          $"businessServices Id: businessServices.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteBusinessServicesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessServices silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessbusinessServicesAdditionalOperationsAsync(Guid businessServicesId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(businessServicesId);
      // await _auditService.LogDeletionAsync(businessServicesId, _currentUserService.UserId);
    }
  }
}