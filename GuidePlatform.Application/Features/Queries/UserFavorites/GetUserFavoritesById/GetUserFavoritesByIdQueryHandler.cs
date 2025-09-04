using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.UserFavorites;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.UserFavorites.GetUserFavoritesById
{
  // Bu handler, bir userFavorites ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetUserFavoritesByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetUserFavoritesByIdQueryRequest, TransactionResultPack<GetUserFavoritesByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetUserFavoritesByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetUserFavoritesByIdQueryResponse>> Handle(GetUserFavoritesByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetUserFavoritesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "userFavorites ID'si belirtilmedi.",
              "userFavorites ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var userFavoritesId = request.GetIdAsGuid();
        if (!userFavoritesId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetUserFavoritesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz userFavorites ID formatı.",
              $"Geçersiz userFavorites ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.userFavorites
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == userFavoritesId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve userFavorites çekiliyor
        var userFavorites = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == userFavoritesId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (userFavorites == null)
        {
          // userFavorites bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetUserFavoritesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / userFavorites Bulunamadı",
              "Belirtilen ID'ye sahip userFavorites bulunamadı.",
              $"ID '{request.Id}' ile eşleşen userFavorites bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (userFavorites.AuthUserId.HasValue)
          allUserIds.Add(userFavorites.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (userFavorites.CreateUserId.HasValue)
          allUserIds.Add(userFavorites.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (userFavorites.UpdateUserId.HasValue)
          allUserIds.Add(userFavorites.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (userFavorites.AuthUserId.HasValue && allUserDetails.ContainsKey(userFavorites.AuthUserId.Value))
        {
          var userDetail = allUserDetails[userFavorites.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (userFavorites.CreateUserId.HasValue && allUserDetails.ContainsKey(userFavorites.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[userFavorites.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (userFavorites.UpdateUserId.HasValue && allUserDetails.ContainsKey(userFavorites.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[userFavorites.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // userFavorites detay DTO'su oluşturuluyor
        var userFavoritesDetail = new UserFavoritesDTO
        {
          Id = userFavorites.Id,
          BusinessId = userFavorites.BusinessId,
          Icon = userFavorites.Icon,
          AuthUserId = userFavorites.AuthUserId,
          AuthCustomerId = userFavorites.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = userFavorites.CreateUserId,
          UpdateUserId = userFavorites.UpdateUserId,
          RowCreatedDate = userFavorites.RowCreatedDate,
          RowUpdatedDate = userFavorites.RowUpdatedDate,
          RowIsActive = userFavorites.RowIsActive,
          RowIsDeleted = userFavorites.RowIsDeleted
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetUserFavoritesByIdQueryResponse>(
            new GetUserFavoritesByIdQueryResponse
            {
              userFavorites = userFavoritesDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "userFavorites başarıyla getirildi.",
            $"userFavorites Id: {userFavorites.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetUserFavoritesByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "userFavorites getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

