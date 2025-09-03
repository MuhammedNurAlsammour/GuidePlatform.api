using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.Businesses.UpdateBusinesses
{
  public class UpdateBusinessesCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBusinessesCommandRequest, TransactionResultPack<UpdateBusinessesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateBusinessesCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateBusinessesCommandResponse>> Handle(UpdateBusinessesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. Businesses bul ve kontrol et - 1. Businesses bul ve kontrol et
        var businesses = await _context.businesses  
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businesses == null)
        {
          return ResultFactory.CreateErrorResult<UpdateBusinessesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek Businesses bulunamadı.",
            "Businesses not found."
          );
        }

        // 🎯 2. Eski durumu kaydet (gerekirse) - 2. Eski durumu kaydet (gerekirse)
        var oldName = businesses.Name;
        var oldDescription = businesses.Description;

        // 🎯 3. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateBusinessesCommandRequest.Map(businesses, request, _currentUserService);

        // 🎯 4. Entity'yi context'e ekle - 4. Entity'yi context'e ekle
        _context.businesses.Update(businesses);

        // 🎯 5. Değişiklikleri kaydet - 5. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 4. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(businesses.Id, request, cancellationToken);

        // 🎯 5. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(businesses.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateBusinessesCommandResponse>(
          new UpdateBusinessesCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "Businesses başarıyla güncellendi.",
          $"Businesses Id: { businesses.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateBusinessesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "Businesses güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid businessesId, UpdateBusinessesCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid businessesId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
