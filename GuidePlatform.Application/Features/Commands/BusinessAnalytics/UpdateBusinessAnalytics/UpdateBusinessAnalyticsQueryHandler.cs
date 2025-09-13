using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.BusinessAnalytics.UpdateBusinessAnalytics
{
  public class UpdateBusinessAnalyticsCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBusinessAnalyticsCommandRequest, TransactionResultPack<UpdateBusinessAnalyticsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateBusinessAnalyticsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateBusinessAnalyticsCommandResponse>> Handle(UpdateBusinessAnalyticsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. businessAnalytics bul ve kontrol et - 1. businessAnalytics bul ve kontrol et
        var businessAnalytics = await _context.businessAnalytics
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessAnalytics == null)
        {
          return ResultFactory.CreateErrorResult<UpdateBusinessAnalyticsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek businessAnalytics bulunamadı.",
            "businessAnalytics not found."
          );
        }

        // 🎯 2. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateBusinessAnalyticsCommandRequest.Map(businessAnalytics, request, _currentUserService);

        // 🎯 3. Entity'yi context'e ekle - 3. Entity'yi context'e ekle
        _context.businessAnalytics.Update(businessAnalytics);

        // 🎯 4. Değişiklikleri kaydet - 4. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 5. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(businessAnalytics.Id, request, cancellationToken);

        // 🎯 6. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(businessAnalytics.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateBusinessAnalyticsCommandResponse>(
          new UpdateBusinessAnalyticsCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "businessAnalytics başarıyla güncellendi.",
          $"businessAnalytics Id: { businessAnalytics.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateBusinessAnalyticsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessAnalytics güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid businessAnalyticsId, UpdateBusinessAnalyticsCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid businessAnalyticsId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
