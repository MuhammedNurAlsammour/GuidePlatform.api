using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Application.Abstractions.Services;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Businesses.IncrementViewCount
{
  public class IncrementViewCountCommandHandler : BaseCommandHandler, IRequestHandler<IncrementViewCountCommandRequest, TransactionResultPack<IncrementViewCountCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public IncrementViewCountCommandHandler(
      IApplicationDbContext context,
      ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<IncrementViewCountCommandResponse>> Handle(IncrementViewCountCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 1. Business'i bul ve kontrol et
        var business = await _context.businesses
          .Where(x => x.Id == request.BusinessId && x.RowIsActive && !x.RowIsDeleted)
          .FirstOrDefaultAsync(cancellationToken);

        if (business == null)
        {
          return ResultFactory.CreateErrorResult<IncrementViewCountCommandResponse>(
            request.BusinessId.ToString(),
            null,
            "Hata / İşlem Başarısız",
            "Business bulunamadı.",
            "Business not found or inactive."
          );
        }

        // 🎯 2. ViewCount'u artır
        business.ViewCount += 1;
        business.RowUpdatedDate = DateTime.UtcNow;

        // Eğer mevcut kullanıcı varsa, güncelleyen kullanıcı bilgisini set et
        var currentUserIdString = _currentUserService.GetUserId();
        if (!string.IsNullOrEmpty(currentUserIdString) && Guid.TryParse(currentUserIdString, out var currentUserId))
        {
          business.UpdateUserId = currentUserId;
        }

        // 🎯 3. Değişiklikleri kaydet
        _context.businesses.Update(business);
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 4. İsteğe bağlı: UserVisits tablosuna kayıt ekle (analytics için)
        await LogUserVisitAsync(request, business.Id, cancellationToken);

        return ResultFactory.CreateSuccessResult<IncrementViewCountCommandResponse>(
          new IncrementViewCountCommandResponse
          {
            BusinessId = business.Id,
            NewViewCount = business.ViewCount,
            ViewedAt = DateTime.UtcNow
          },
          null,
          null,
          "İşlem Başarılı",
          "Görüntüleme sayısı başarıyla artırıldı.",
          $"Business {business.Name} görüntüleme sayısı: {business.ViewCount}"
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<IncrementViewCountCommandResponse>(
          request.BusinessId.ToString(),
          null,
          "Hata / İşlem Başarısız",
          "Görüntüleme sayısı artırılırken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// İsteğe bağlı: Kullanıcı ziyaretini UserVisits tablosuna kaydet (analytics için)
    /// </summary>
    private async Task LogUserVisitAsync(IncrementViewCountCommandRequest request, Guid businessId, CancellationToken cancellationToken)
    {
      try
      {
        // UserVisits entity'si varsa burada kayıt oluştur
        // Örnek implementasyon:
        /*
        var userVisit = new UserVisitsViewModel
        {
          Id = Guid.NewGuid(),
          BusinessId = businessId,
          AuthUserId = _currentUserService.GetUserId(),
          AuthCustomerId = _currentUserService.GetCustomerId(),
          IpAddress = request.IpAddress,
          UserAgent = request.UserAgent,
          RefererUrl = request.RefererUrl,
          VisitedAt = DateTime.UtcNow,
          RowCreatedDate = DateTime.UtcNow,
          RowUpdatedDate = DateTime.UtcNow,
          RowIsActive = true,
          RowIsDeleted = false
        };

        _context.userVisits.Add(userVisit);
        await _context.SaveChangesAsync(cancellationToken);
        */
      }
      catch (Exception ex)
      {
        // Log error ama ana işlemi etkileme
        // _logger.LogWarning($"UserVisit kaydedilemedi: {ex.Message}");
      }
    }
  }
}
