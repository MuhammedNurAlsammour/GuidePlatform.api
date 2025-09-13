using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Banners;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Banners.GetBannersById
{
  // Bu handler, bir banners ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetBannersByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBannersByIdQueryRequest, TransactionResultPack<GetBannersByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetBannersByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetBannersByIdQueryResponse>> Handle(GetBannersByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBannersByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "banners ID'si belirtilmedi.",
              "banners ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var bannersId = request.GetIdAsGuid();
        var provinceId = request.GetIdAsGuid();
        if (!bannersId.HasValue || !provinceId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBannersByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz banners ID formatı.",
              $"Geçersiz banners ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }
        if (request.ProvinceId.HasValue)
        {
          provinceId = request.ProvinceId.Value;
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.banners
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == bannersId.Value && x.ProvinceId == provinceId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve banners çekiliyor
        var banners = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == bannersId.Value && x.ProvinceId == provinceId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (banners == null)
        {
          // banners bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBannersByIdQueryResponse>(
              request.Id,
              null,
              "Hata / banners Bulunamadı",
              "Belirtilen ID'ye sahip banners bulunamadı.",
              $"ID '{request.Id}' ve '{request.ProvinceId}' ile eşleşen banners bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (banners.AuthUserId.HasValue)
          allUserIds.Add(banners.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (banners.CreateUserId.HasValue)
          allUserIds.Add(banners.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (banners.UpdateUserId.HasValue)
          allUserIds.Add(banners.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (banners.AuthUserId.HasValue && allUserDetails.ContainsKey(banners.AuthUserId.Value))
        {
          var userDetail = allUserDetails[banners.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (banners.CreateUserId.HasValue && allUserDetails.ContainsKey(banners.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[banners.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (banners.UpdateUserId.HasValue && allUserDetails.ContainsKey(banners.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[banners.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // banners detay DTO'su oluşturuluyor
        var bannersDetail = new BannersDTO
        {
          Id = banners.Id,
          AuthUserId = banners.AuthUserId,
          AuthCustomerId = banners.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = banners.CreateUserId,
          UpdateUserId = banners.UpdateUserId,
          RowCreatedDate = banners.RowCreatedDate,
          RowUpdatedDate = banners.RowUpdatedDate,
          RowIsActive = banners.RowIsActive,
          RowIsDeleted = banners.RowIsDeleted,
          // Banner özel alanları - Banner specific fields
          Title = banners.Title,
          Description = banners.Description,
          ProvinceId = banners.ProvinceId,
          // Yeni sistem: URL'leri kullan - New system: Use URLs
          PhotoUrl = banners.PhotoUrl,
          ThumbnailUrl = banners.ThumbnailUrl,
          // Eski sistem: Base64'ü koru (geriye dönük uyumluluk için) - Old system: Keep Base64 (for backward compatibility)
          Photo = banners.Photo,
          Thumbnail = banners.Thumbnail,
          PhotoContentType = banners.PhotoContentType,
          LinkUrl = banners.LinkUrl,
          StartDate = banners.StartDate,
          EndDate = banners.EndDate,
          IsActive = banners.IsActive,
          OrderIndex = banners.OrderIndex,
          Icon = banners.Icon
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetBannersByIdQueryResponse>(
            new GetBannersByIdQueryResponse
            {
              banners = bannersDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "banners başarıyla getirildi.",
            $"banners Id: {banners.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetBannersByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "banners getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

