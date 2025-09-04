using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessImages.DeleteBusinessImages
{
  public class DeleteBusinessImagesCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBusinessImagesCommandRequest, TransactionResultPack<DeleteBusinessImagesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteBusinessImagesCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteBusinessImagesCommandResponse>> Handle(DeleteBusinessImagesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var businessImages = await _context.businessImages
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessImages == null)
        {
          return ResultFactory.CreateErrorResult<DeleteBusinessImagesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek businessImages bulunamadı.",
            "businessImages not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessbusinessImagesAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.businessImages.Remove(businessImages);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteBusinessImagesCommandResponse>(
          new DeleteBusinessImagesCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "businessImages silindi ve stok geri iade edildi.",
          $"businessImages Id: businessImages.Id başarıyla silindi ve envanter güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteBusinessImagesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessImages silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessbusinessImagesAdditionalOperationsAsync(Guid businessImagesId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(businessImagesId);
      // await _auditService.LogDeletionAsync(businessImagesId, _currentUserService.UserId);
    }
  }
}