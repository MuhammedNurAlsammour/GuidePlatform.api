using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Businesses.DeleteBusinesses
{
  public class DeleteBusinessesCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBusinessesCommandRequest, TransactionResultPack<DeleteBusinessesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteBusinessesCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteBusinessesCommandResponse>> Handle(DeleteBusinessesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var businesses = await _context.businesses
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businesses == null)
        {
          return ResultFactory.CreateErrorResult<DeleteBusinessesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek businesses bulunamadı.",
            "businesses not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessbusinessesAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.businesses.Remove(businesses);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteBusinessesCommandResponse>(
          new DeleteBusinessesCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "businesses silindi ve stok geri iade edildi.",
          $"businesses Id: businesses.Id başarıyla silindi ve envanter güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteBusinessesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businesses silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessbusinessesAdditionalOperationsAsync(Guid businessesId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma

      // await _emailService.SendDeletionNotificationAsync(businessesId);
      // await _auditService.LogDeletionAsync(businessesId, _currentUserService.UserId);
    }
  }
}