using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessReviews.CreateBusinessReviews
{
  public class CreateBusinessReviewsCommandHandler : BaseCommandHandler, IRequestHandler<CreateBusinessReviewsCommandRequest, TransactionResultPack<CreateBusinessReviewsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public CreateBusinessReviewsCommandHandler(
        IApplicationDbContext context,
        IMediator mediator,
        ICurrentUserService currentUserService
    ) : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<CreateBusinessReviewsCommandResponse>> Handle(CreateBusinessReviewsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // Kullanıcı bilgilerini kontrol et
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 1. businessReviews oluştur
        var businessReviews = CreateBusinessReviewsCommandRequest.Map(request, _currentUserService);
        await _context.businessReviews.AddAsync(businessReviews, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Business'in TotalReviews ve Rating değerlerini güncelle
        await UpdateBusinessRatingAndReviewCount(businessReviews.BusinessId, cancellationToken);

        return ResultFactory.CreateSuccessResult<CreateBusinessReviewsCommandResponse>(
            new CreateBusinessReviewsCommandResponse
            {
              StatusCode = (int)HttpStatusCode.Created,
              Id = businessReviews.Id
            },
            null,
            null,
            "İşlem Başarılı",
            "businessReviews başarıyla oluşturuldu.",
            $"businessReviews Id: {businessReviews.Id} başarıyla oluşturuldu."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<CreateBusinessReviewsCommandResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "businessReviews oluşturulurken bir hata oluştu.",
            ex.InnerException?.Message ?? ex.Message
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
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid businessReviewsId, CreateBusinessReviewsCommandRequest request, CancellationToken cancellationToken)
    // {
    //     // Ek işlemler buraya eklenebilir
    //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
    // }
  }
}
