using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.UserFavorites.UpdateUserFavorites
{
  public class UpdateUserFavoritesCommandHandler : BaseCommandHandler, IRequestHandler<UpdateUserFavoritesCommandRequest, TransactionResultPack<UpdateUserFavoritesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateUserFavoritesCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateUserFavoritesCommandResponse>> Handle(UpdateUserFavoritesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. userFavorites bul ve kontrol et - 1. userFavorites bul ve kontrol et
        var userFavorites = await _context.userFavorites
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (userFavorites == null)
        {
          return ResultFactory.CreateErrorResult<UpdateUserFavoritesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek userFavorites bulunamadı.",
            "userFavorites not found."
          );
        }

        // 🎯 3. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateUserFavoritesCommandRequest.Map(userFavorites, request, _currentUserService);

        // 🎯 4. Entity'yi context'e ekle - 4. Entity'yi context'e ekle
        _context.userFavorites.Update(userFavorites);

        // 🎯 5. Değişiklikleri kaydet - 5. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 4. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(userFavorites.Id, request, cancellationToken);

        // 🎯 5. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(userFavorites.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateUserFavoritesCommandResponse>(
          new UpdateUserFavoritesCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "userFavorites başarıyla güncellendi.",
          $"userFavorites Id: { userFavorites.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateUserFavoritesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "userFavorites güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid userFavoritesId, UpdateUserFavoritesCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid userFavoritesId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
