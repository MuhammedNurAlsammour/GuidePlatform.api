using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Businesses;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Businesses.GetBusinessesById
{
  // Bu handler, bir Businesses ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetBusinessesByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBusinessesByIdQueryRequest, TransactionResultPack<GetBusinessesByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetBusinessesByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetBusinessesByIdQueryResponse>> Handle(GetBusinessesByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "Businesses ID'si belirtilmedi.",
              "Businesses ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var businessesId = request.GetIdAsGuid();
        if (!businessesId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz Businesses ID formatı.",
              $"Geçersiz Businesses ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.businesses
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == businessesId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve Businesses çekiliyor
        var businesses = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == businessesId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (businesses == null)
        {
          // Businesses bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Businesses Bulunamadı",
              "Belirtilen ID'ye sahip Businesses bulunamadı.",
              $"ID '{request.Id}' ile eşleşen Businesses bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (businesses.AuthUserId.HasValue)
          allUserIds.Add(businesses.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (businesses.CreateUserId.HasValue)
          allUserIds.Add(businesses.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (businesses.UpdateUserId.HasValue)
          allUserIds.Add(businesses.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (businesses.AuthUserId.HasValue && allUserDetails.ContainsKey(businesses.AuthUserId.Value))
        {
          var userDetail = allUserDetails[businesses.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (businesses.CreateUserId.HasValue && allUserDetails.ContainsKey(businesses.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[businesses.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (businesses.UpdateUserId.HasValue && allUserDetails.ContainsKey(businesses.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[businesses.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // Businesses detay DTO'su oluşturuluyor
        var businessesDetail = new BusinessesDTO
        {
          Id = businesses.Id,
          AuthUserId = businesses.AuthUserId,
          AuthCustomerId = businesses.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = businesses.CreateUserId,
          UpdateUserId = businesses.UpdateUserId,
          RowCreatedDate = businesses.RowCreatedDate,
          RowUpdatedDate = businesses.RowUpdatedDate,
          RowIsActive = businesses.RowIsActive,
          RowIsDeleted = businesses.RowIsDeleted,


          // 🏢 Temel iş bilgileri - Basic business information
          Name = businesses.Name,
          Description = businesses.Description,
          CategoryId = businesses.CategoryId,
          SubCategoryId = businesses.SubCategoryId,

          // 📍 Konum bilgileri - Location information
          ProvinceId = businesses.ProvinceId,
          CountriesId = businesses.CountriesId,
          DistrictId = businesses.DistrictId,
          Address = businesses.Address,
          Latitude = businesses.Latitude,
          Longitude = businesses.Longitude,

          // 📞 İletişim bilgileri - Contact information
          Phone = businesses.Phone,
          Mobile = businesses.Mobile,
          Email = businesses.Email,
          Website = businesses.Website,
          FacebookUrl = businesses.FacebookUrl,
          InstagramUrl = businesses.InstagramUrl,
          WhatsApp = businesses.WhatsApp,
          Telegram = businesses.Telegram,

          // ⭐ Değerlendirme ve istatistikler - Rating and statistics
          Rating = businesses.Rating,
          TotalReviews = businesses.TotalReviews,
          ViewCount = businesses.ViewCount,

          // 💼 İş özellikleri - Business features
          SubscriptionType = businesses.SubscriptionType,
          IsVerified = businesses.IsVerified,
          IsFeatured = businesses.IsFeatured,
          WorkingHours = businesses.WorkingHours,
          Icon = businesses.Icon,

          // 👤 Sahiplik bilgileri - Ownership information
          OwnerId = businesses.OwnerId
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetBusinessesByIdQueryResponse>(
            new GetBusinessesByIdQueryResponse
            {
              TotalCount = totalCount,
              businesses = businessesDetail
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "Businesses başarıyla getirildi.",
            $"Businesses Id: {businesses.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetBusinessesByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Businesses getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

