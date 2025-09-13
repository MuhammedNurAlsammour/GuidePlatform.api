using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessReviews.DeleteBusinessReviews
{
  public class DeleteBusinessReviewsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBusinessReviewsCommandRequest, TransactionResultPack<DeleteBusinessReviewsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteBusinessReviewsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteBusinessReviewsCommandResponse>> Handle(DeleteBusinessReviewsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var businessReviewss = await _context.businessReviews
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessReviewss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteBusinessReviewsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek businessReviews bulunamadı.",
            "businessReviews not found."
          );
        }

        // BusinessId'yi kaydet (silmeden önce)
        var businessId = businessReviewss.BusinessId;

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessbusinessReviewsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.businessReviews.Remove(businessReviewss);
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 Business'in TotalReviews ve Rating değerlerini güncelle
        await UpdateBusinessRatingAndReviewCount(businessId, cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteBusinessReviewsCommandResponse>(
          new DeleteBusinessReviewsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "businessReviews silindi.",
          $"businessReviews Id: businessReviews.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteBusinessReviewsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessReviews silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Business'in TotalReviews ve Rating değerlerini günceller
    /// </summary>
    private async Task UpdateBusinessRatingAndReviewCount(Guid businessId, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 1. Business'i bul
        var business = await _context.businesses
            .Where(x => x.Id == businessId && x.RowIsActive && !x.RowIsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (business == null) return; // Business bulunamazsa çık

        // 🎯 2. Bu Business için onaylanmış ve aktif tüm yorumları al
        var approvedReviews = await _context.businessReviews
            .Where(x => x.BusinessId == businessId &&
                       x.IsApproved &&
                       x.RowIsActive &&
                       !x.RowIsDeleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // 🎯 3. TotalReviews hesapla
        business.TotalReviews = approvedReviews.Count;

        // 🎯 4. Rating ortalamasını hesapla
        if (approvedReviews.Any())
        {
          business.Rating = (decimal)approvedReviews.Average(x => x.Rating);
        }
        else
        {
          business.Rating = 0; // Hiç yorum yoksa 0
        }

        // 🎯 5. Güncelleme bilgilerini set et
        business.RowUpdatedDate = DateTime.UtcNow;

        var currentUserIdString = _currentUserService.GetUserId();
        if (!string.IsNullOrEmpty(currentUserIdString) && Guid.TryParse(currentUserIdString, out var currentUserId))
        {
          business.UpdateUserId = currentUserId;
        }

        // 🎯 6. Değişiklikleri kaydet
        _context.businesses.Update(business);
        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception ex)
      {
        // Log error ama ana işlemi etkileme
        // _logger.LogWarning($"Business rating güncellenemedi: {ex.Message}");
      }
    }

    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessbusinessReviewsAdditionalOperationsAsync(Guid businessReviewsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma

      // await _emailService.SendDeletionNotificationAsync(businessReviewsId);
      // await _auditService.LogDeletionAsync(businessReviewsId, _currentUserService.UserId);
    }
  }
}