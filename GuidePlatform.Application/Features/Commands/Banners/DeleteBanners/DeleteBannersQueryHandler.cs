using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Banners.DeleteBanners
{
  public class DeleteBannersCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBannersCommandRequest, TransactionResultPack<DeleteBannersCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteBannersCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteBannersCommandResponse>> Handle(DeleteBannersCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var bannerss = await _context.banners
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (bannerss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteBannersCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek banners bulunamadı.",
            "banners not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessbannersAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.banners.Remove(bannerss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteBannersCommandResponse>(
          new DeleteBannersCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "banners silindi.",
          $"banners Id: banners.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteBannersCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "banners silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessbannersAdditionalOperationsAsync(Guid bannersId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(bannersId);
      // await _auditService.LogDeletionAsync(bannersId, _currentUserService.UserId);
    }
  }
}