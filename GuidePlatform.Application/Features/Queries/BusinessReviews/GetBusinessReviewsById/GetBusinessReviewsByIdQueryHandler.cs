using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetBusinessReviewsById
{
  // Bu handler, bir businessReviews ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetBusinessReviewsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBusinessReviewsByIdQueryRequest, TransactionResultPack<GetBusinessReviewsByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetBusinessReviewsByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetBusinessReviewsByIdQueryResponse>> Handle(GetBusinessReviewsByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessReviewsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "businessReviews ID'si belirtilmedi.",
              "businessReviews ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var businessReviewsId = request.GetIdAsGuid();
        if (!businessReviewsId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessReviewsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz businessReviews ID formatı.",
              $"Geçersiz businessReviews ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.businessReviews
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == businessReviewsId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve businessReviews çekiliyor
        var businessReviews = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == businessReviewsId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (businessReviews == null)
        {
          // businessReviews bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessReviewsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / businessReviews Bulunamadı",
              "Belirtilen ID'ye sahip businessReviews bulunamadı.",
              $"ID '{request.Id}' ile eşleşen businessReviews bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (businessReviews.AuthUserId.HasValue)
          allUserIds.Add(businessReviews.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (businessReviews.CreateUserId.HasValue)
          allUserIds.Add(businessReviews.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (businessReviews.UpdateUserId.HasValue)
          allUserIds.Add(businessReviews.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (businessReviews.AuthUserId.HasValue && allUserDetails.ContainsKey(businessReviews.AuthUserId.Value))
        {
          var userDetail = allUserDetails[businessReviews.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

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

        // 🎯 BusinessName bilgisini al
        string? businessName = null;
        var business = await _context.businesses
            .Where(b => b.Id == businessReviews.BusinessId && b.RowIsActive && !b.RowIsDeleted)
            .Select(b => b.Name)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        businessName = business ?? "Unknown Business";

        // 🎯 ReviewerName bilgisini al (NameSurname)
        string? reviewerName = null;
        var reviewerDetails = await _authUserService.GetAuthUserDetailsAsync(new List<Guid> { businessReviews.ReviewerId }, cancellationToken);
        if (reviewerDetails.ContainsKey(businessReviews.ReviewerId))
        {
          var reviewerDetail = reviewerDetails[businessReviews.ReviewerId];
          reviewerName = reviewerDetail.AuthUserFullName ?? reviewerDetail.AuthUserName ?? "Unknown User";
        }
        else
        {
          reviewerName = "Unknown User";
        }

        // businessReviews detay DTO'su oluşturuluyor
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
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = businessReviews.CreateUserId,
          UpdateUserId = businessReviews.UpdateUserId,
          RowCreatedDate = businessReviews.RowCreatedDate,
          RowUpdatedDate = businessReviews.RowUpdatedDate,
          RowIsActive = businessReviews.RowIsActive,
          RowIsDeleted = businessReviews.RowIsDeleted
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetBusinessReviewsByIdQueryResponse>(
            new GetBusinessReviewsByIdQueryResponse
            {
              businessReviews = businessReviewsDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "businessReviews başarıyla getirildi.",
            $"businessReviews Id: {businessReviews.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetBusinessReviewsByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "businessReviews getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

