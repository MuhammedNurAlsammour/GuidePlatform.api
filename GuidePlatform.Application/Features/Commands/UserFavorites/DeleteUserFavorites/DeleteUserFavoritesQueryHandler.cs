using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.UserFavorites.DeleteUserFavorites
{
  public class DeleteUserFavoritesCommandHandler : BaseCommandHandler, IRequestHandler<DeleteUserFavoritesCommandRequest, TransactionResultPack<DeleteUserFavoritesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteUserFavoritesCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteUserFavoritesCommandResponse>> Handle(DeleteUserFavoritesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var userFavoritess = await _context.userFavorites
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (userFavoritess == null)
        {
          return ResultFactory.CreateErrorResult<DeleteUserFavoritesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek userFavorites bulunamadı.",
            "userFavorites not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessuserFavoritesAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.userFavorites.Remove(userFavoritess);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteUserFavoritesCommandResponse>(
          new DeleteUserFavoritesCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "userFavorites silindi.",
          $"userFavorites Id: userFavorites.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteUserFavoritesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "userFavorites silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessuserFavoritesAdditionalOperationsAsync(Guid userFavoritesId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(userFavoritesId);
      // await _auditService.LogDeletionAsync(userFavoritesId, _currentUserService.UserId);
    }
  }
}