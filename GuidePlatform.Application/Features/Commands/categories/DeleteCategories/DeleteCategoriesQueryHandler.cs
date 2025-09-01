using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.categories.DeleteCategories
{
  public class DeleteCategoriesCommandHandler : BaseCommandHandler, IRequestHandler<DeleteCategoriesCommandRequest, TransactionResultPack<DeleteCategoriesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteCategoriesCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteCategoriesCommandResponse>> Handle(DeleteCategoriesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var categories = await _context.categories
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (categories == null)
        {
          return ResultFactory.CreateErrorResult<DeleteCategoriesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek categories bulunamadı.",
            "categories not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcesscategoriesAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.categories.Remove(categories);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteCategoriesCommandResponse>(
          new DeleteCategoriesCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "categories silindi ve stok geri iade edildi.",
          $"categories Id: categories.Id başarıyla silindi ve envanter güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteCategoriesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "categories silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcesscategoriesAdditionalOperationsAsync(Guid categoriesId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(categoriesId);
      // await _auditService.LogDeletionAsync(categoriesId, _currentUserService.UserId);
    }
  }
}