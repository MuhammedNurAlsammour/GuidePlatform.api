using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessAnalytics;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetBusinessAnalyticsById
{
  // Bu handler, bir businessAnalytics ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetBusinessAnalyticsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBusinessAnalyticsByIdQueryRequest, TransactionResultPack<GetBusinessAnalyticsByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetBusinessAnalyticsByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetBusinessAnalyticsByIdQueryResponse>> Handle(GetBusinessAnalyticsByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessAnalyticsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "businessAnalytics ID'si belirtilmedi.",
              "businessAnalytics ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var businessAnalyticsId = request.GetIdAsGuid();
        if (!businessAnalyticsId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessAnalyticsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz businessAnalytics ID formatı.",
              $"Geçersiz businessAnalytics ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.businessAnalytics
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == businessAnalyticsId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve businessAnalytics çekiliyor
        var businessAnalytics = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == businessAnalyticsId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (businessAnalytics == null)
        {
          // businessAnalytics bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessAnalyticsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / businessAnalytics Bulunamadı",
              "Belirtilen ID'ye sahip businessAnalytics bulunamadı.",
              $"ID '{request.Id}' ile eşleşen businessAnalytics bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (businessAnalytics.AuthUserId.HasValue)
          allUserIds.Add(businessAnalytics.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (businessAnalytics.CreateUserId.HasValue)
          allUserIds.Add(businessAnalytics.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (businessAnalytics.UpdateUserId.HasValue)
          allUserIds.Add(businessAnalytics.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (businessAnalytics.AuthUserId.HasValue && allUserDetails.ContainsKey(businessAnalytics.AuthUserId.Value))
        {
          var userDetail = allUserDetails[businessAnalytics.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
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

        // businessAnalytics detay DTO'su oluşturuluyor
        var businessAnalyticsDetail = new BusinessAnalyticsDTO
        {
          Id = businessAnalytics.Id,
          AuthUserId = businessAnalytics.AuthUserId,
          AuthCustomerId = businessAnalytics.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = businessAnalytics.CreateUserId,
          UpdateUserId = businessAnalytics.UpdateUserId,
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

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetBusinessAnalyticsByIdQueryResponse>(
            new GetBusinessAnalyticsByIdQueryResponse
            {
              businessAnalytics = businessAnalyticsDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "businessAnalytics başarıyla getirildi.",
            $"businessAnalytics Id: {businessAnalytics.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetBusinessAnalyticsByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "businessAnalytics getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

