using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetAllBusinessReviews
{
  public class GetAllBusinessReviewsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllBusinessReviewsQueryRequest, TransactionResultPack<GetAllBusinessReviewsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllBusinessReviewsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllBusinessReviewsQueryResponse>> Handle(GetAllBusinessReviewsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.businessReviews
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // Filtreleme uygula
        if (!string.IsNullOrWhiteSpace(request.BusinessId) && Guid.TryParse(request.BusinessId, out var businessId))
        {
          baseQuery = baseQuery.Where(x => x.BusinessId == businessId);
        }

        if (!string.IsNullOrWhiteSpace(request.ReviewerId) && Guid.TryParse(request.ReviewerId, out var reviewerId))
        {
          baseQuery = baseQuery.Where(x => x.ReviewerId == reviewerId);
        }

        if (request.IsApproved.HasValue)
        {
          baseQuery = baseQuery.Where(x => x.IsApproved == request.IsApproved.Value);
        }

        if (request.IsVerified.HasValue)
        {
          baseQuery = baseQuery.Where(x => x.IsVerified == request.IsVerified.Value);
        }

        if (request.MinRating.HasValue)
        {
          baseQuery = baseQuery.Where(x => x.Rating >= request.MinRating.Value);
        }

        if (request.MaxRating.HasValue)
        {
          baseQuery = baseQuery.Where(x => x.Rating <= request.MaxRating.Value);
        }

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var businessReviewss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Base Handler'ın ExtractAuthUserDetailsAsync method'unu kullan
        var allUserDetails = await ExtractAuthUserDetailsAsync(businessReviewss, cancellationToken);

        // 🎯 Business isimlerini toplu olarak al (performans için)
        var businessIds = businessReviewss.Select(br => br.BusinessId).Distinct().ToList();
        var businessNames = await _context.businesses
            .Where(b => businessIds.Contains(b.Id) && b.RowIsActive && !b.RowIsDeleted)
            .Select(b => new { b.Id, b.Name })
            .AsNoTracking()
            .ToDictionaryAsync(b => b.Id, b => b.Name, cancellationToken);

        // 🎯 Reviewer isimlerini toplu olarak al (performans için) - AspNetUsers'dan NameSurname
        var reviewerIds = businessReviewss.Select(br => br.ReviewerId).Distinct().ToList();
        var reviewerNames = new Dictionary<Guid, string>();

        if (reviewerIds.Any())
        {
          // AuthUserService'den reviewer bilgilerini al
          var reviewerDetails = await _authUserService.GetAuthUserDetailsAsync(reviewerIds, cancellationToken);
          reviewerNames = reviewerDetails.ToDictionary(
              kvp => kvp.Key,
              kvp => kvp.Value.AuthUserFullName ?? kvp.Value.AuthUserName ?? "Unknown User"
          );
        }

        var businessReviewsDetails = new List<BusinessReviewsDTO>();  // 🎯 businessReviewsDTO listesi oluştur

        foreach (var businessReviews in businessReviewss)
        {
          // 🎯 Base Handler'ın GetAuthUserInfo method'unu kullan
          var (authUserName, authCustomerName) = GetAuthUserInfo(businessReviews, allUserDetails);

          // 🎯 Create/Update kullanıcı bilgilerini al
          string? createUserName = null;
          string? updateUserName = null;

          if (businessReviews.CreateUserId.HasValue && allUserDetails.ContainsKey(businessReviews.CreateUserId.Value))
          {
            var createUserDetail = allUserDetails[businessReviews.CreateUserId.Value];
            createUserName = createUserDetail.AuthUserName;
          }

          if (businessReviews.UpdateUserId.HasValue && allUserDetails.ContainsKey(businessReviews.UpdateUserId.Value))
          {
            var updateUserDetail = allUserDetails[businessReviews.UpdateUserId.Value];
            updateUserName = updateUserDetail.AuthUserName;
          }

          // 🎯 BusinessName ve ReviewerName bilgilerini al
          var businessName = businessNames.GetValueOrDefault(businessReviews.BusinessId, "Unknown Business");
          var reviewerName = reviewerNames.GetValueOrDefault(businessReviews.ReviewerId, "Unknown User");

          var businessReviewsDetail = new BusinessReviewsDTO
          {
            Id = businessReviews.Id,
            BusinessId = businessReviews.BusinessId,
            BusinessName = businessName, // Business adı
            ReviewerId = businessReviews.ReviewerId,
            ReviewerName = reviewerName, // Reviewer NameSurname
            Rating = businessReviews.Rating,
            Comment = businessReviews.Comment,
            IsVerified = businessReviews.IsVerified,
            IsApproved = businessReviews.IsApproved,
            Icon = businessReviews.Icon,
            AuthUserId = businessReviews.AuthUserId,
            AuthCustomerId = businessReviews.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = businessReviews.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = businessReviews.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = businessReviews.RowCreatedDate,
            RowUpdatedDate = businessReviews.RowUpdatedDate,
            RowIsActive = businessReviews.RowIsActive,
            RowIsDeleted = businessReviews.RowIsDeleted
          };

          businessReviewsDetails.Add(businessReviewsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllBusinessReviewsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllBusinessReviewsQueryResponse
            {
              TotalCount = totalCount,
              businessReviews = businessReviewsDetails  // 🎯 businessReviewsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "businessReviews başarıyla getirildi.",
            $"businessReviewss.Count businessReviews başarıyla getirildi."  // 🎯 businessReviews sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllBusinessReviewsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "businessReviews getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}
