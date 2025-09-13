using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessAnalytics;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetAllBusinessAnalytics
{
  public class GetAllBusinessAnalyticsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllBusinessAnalyticsQueryRequest, TransactionResultPack<GetAllBusinessAnalyticsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllBusinessAnalyticsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllBusinessAnalyticsQueryResponse>> Handle(GetAllBusinessAnalyticsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.businessAnalytics
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyBusinessAnalyticsFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var businessAnalyticss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = businessAnalyticss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = businessAnalyticss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = businessAnalyticss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var businessAnalyticsDetails = new List<BusinessAnalyticsDTO>();  // 🎯 businessAnalyticsDTO listesi oluştur

        foreach (var businessAnalytics in businessAnalyticss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (businessAnalytics.AuthUserId.HasValue && allUserDetails.ContainsKey(businessAnalytics.AuthUserId.Value))
          {
            var userDetail = allUserDetails[businessAnalytics.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
          }

          // 🎯 Create/Update kullanıcı bilgilerini al
          string? createUserName = null;
          string? updateUserName = null;

          if (businessAnalytics.CreateUserId.HasValue && allUserDetails.ContainsKey(businessAnalytics.CreateUserId.Value))
          {
            var createUserDetail = allUserDetails[businessAnalytics.CreateUserId.Value];
            createUserName = createUserDetail.AuthUserName;
          }

          if (businessAnalytics.UpdateUserId.HasValue && allUserDetails.ContainsKey(businessAnalytics.UpdateUserId.Value))
          {
            var updateUserDetail = allUserDetails[businessAnalytics.UpdateUserId.Value];
            updateUserName = updateUserDetail.AuthUserName;
          }

          var businessAnalyticsDetail = new BusinessAnalyticsDTO
          {
            Id = businessAnalytics.Id,
            AuthUserId = businessAnalytics.AuthUserId,
            AuthCustomerId = businessAnalytics.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = businessAnalytics.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = businessAnalytics.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = businessAnalytics.RowCreatedDate,
            RowUpdatedDate = businessAnalytics.RowUpdatedDate,
            RowIsActive = businessAnalytics.RowIsActive,
            RowIsDeleted = businessAnalytics.RowIsDeleted,
            // İş analitik verileri
            BusinessId = businessAnalytics.BusinessId,
            Date = businessAnalytics.Date,
            ViewsCount = businessAnalytics.ViewsCount,
            ContactsCount = businessAnalytics.ContactsCount,
            ReviewsCount = businessAnalytics.ReviewsCount,
            FavoritesCount = businessAnalytics.FavoritesCount,
            Icon = businessAnalytics.Icon
          };

          businessAnalyticsDetails.Add(businessAnalyticsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllBusinessAnalyticsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllBusinessAnalyticsQueryResponse
            {
              TotalCount = totalCount,
              businessAnalytics = businessAnalyticsDetails  // 🎯 businessAnalyticsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "businessAnalytics başarıyla getirildi.",
            $"businessAnalyticss.Count businessAnalytics başarıyla getirildi."  // 🎯 businessAnalytics sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllBusinessAnalyticsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "businessAnalytics getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı BusinessAnalytics filtrelerini uygular - Applies BusinessAnalytics filters
    /// </summary>
    private IQueryable<BusinessAnalyticsViewModel> ApplyBusinessAnalyticsFilters(
        IQueryable<BusinessAnalyticsViewModel> query,
        GetAllBusinessAnalyticsQueryRequest request)
    {
      // İş ID'sine göre filtreleme - Filter by Business ID
      if (request.BusinessId.HasValue)
      {
        query = query.Where(x => x.BusinessId == request.BusinessId.Value);
      }

      // Tarih aralığına göre filtreleme - Filter by date range
      if (request.StartDate.HasValue)
      {
        query = query.Where(x => x.Date >= request.StartDate.Value.Date);
      }

      if (request.EndDate.HasValue)
      {
        query = query.Where(x => x.Date <= request.EndDate.Value.Date);
      }

      // Görüntülenme sayısına göre filtreleme - Filter by views count
      if (request.MinViewsCount.HasValue)
      {
        query = query.Where(x => x.ViewsCount >= request.MinViewsCount.Value);
      }

      if (request.MaxViewsCount.HasValue)
      {
        query = query.Where(x => x.ViewsCount <= request.MaxViewsCount.Value);
      }

      // İletişim sayısına göre filtreleme - Filter by contacts count
      if (request.MinContactsCount.HasValue)
      {
        query = query.Where(x => x.ContactsCount >= request.MinContactsCount.Value);
      }

      if (request.MaxContactsCount.HasValue)
      {
        query = query.Where(x => x.ContactsCount <= request.MaxContactsCount.Value);
      }

      // Yorum sayısına göre filtreleme - Filter by reviews count
      if (request.MinReviewsCount.HasValue)
      {
        query = query.Where(x => x.ReviewsCount >= request.MinReviewsCount.Value);
      }

      if (request.MaxReviewsCount.HasValue)
      {
        query = query.Where(x => x.ReviewsCount <= request.MaxReviewsCount.Value);
      }

      // Favori sayısına göre filtreleme - Filter by favorites count
      if (request.MinFavoritesCount.HasValue)
      {
        query = query.Where(x => x.FavoritesCount >= request.MinFavoritesCount.Value);
      }

      if (request.MaxFavoritesCount.HasValue)
      {
        query = query.Where(x => x.FavoritesCount <= request.MaxFavoritesCount.Value);
      }

      // İkon türüne göre filtreleme - Filter by icon type
      if (!string.IsNullOrEmpty(request.Icon))
      {
        query = query.Where(x => x.Icon == request.Icon);
      }

      // Oluşturma kullanıcı ID'sine göre filtreleme - Filter by create user ID
      if (request.CreateUserId.HasValue)
      {
        query = query.Where(x => x.CreateUserId == request.CreateUserId.Value);
      }

      // Güncelleme kullanıcı ID'sine göre filtreleme - Filter by update user ID
      if (request.UpdateUserId.HasValue)
      {
        query = query.Where(x => x.UpdateUserId == request.UpdateUserId.Value);
      }

      // Aktif kayıtları filtreleme - Filter active records
      if (request.OnlyActive.HasValue && request.OnlyActive.Value)
      {
        query = query.Where(x => x.RowIsActive);
      }

      // Silinmemiş kayıtları filtreleme - Filter non-deleted records
      if (request.OnlyNonDeleted.HasValue && request.OnlyNonDeleted.Value)
      {
        query = query.Where(x => !x.RowIsDeleted);
      }

      return query;
    }
  }
}
