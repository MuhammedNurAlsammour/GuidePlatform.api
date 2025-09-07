using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.UserFavorites;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.UserFavorites.GetAllUserFavorites
{
  public class GetAllUserFavoritesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllUserFavoritesQueryRequest, TransactionResultPack<GetAllUserFavoritesQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllUserFavoritesQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllUserFavoritesQueryResponse>> Handle(GetAllUserFavoritesQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.userFavorites
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyUserFavoritesFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var userFavoritess = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = userFavoritess.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = userFavoritess.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = userFavoritess.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var userFavoritesDetails = new List<UserFavoritesDTO>();  // 🎯 userFavoritesDTO listesi oluştur

        foreach (var userFavorites in userFavoritess)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (userFavorites.AuthUserId.HasValue && allUserDetails.ContainsKey(userFavorites.AuthUserId.Value))
          {
            var userDetail = allUserDetails[userFavorites.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var userFavoritesDetail = new UserFavoritesDTO
          {
            Id = userFavorites.Id,
            BusinessId = userFavorites.BusinessId,
            Icon = userFavorites.Icon,
            AuthUserId = userFavorites.AuthUserId,
            AuthCustomerId = userFavorites.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = userFavorites.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = userFavorites.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = userFavorites.RowCreatedDate,
            RowUpdatedDate = userFavorites.RowUpdatedDate,
            RowIsActive = userFavorites.RowIsActive,
            RowIsDeleted = userFavorites.RowIsDeleted
          };

          userFavoritesDetails.Add(userFavoritesDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllUserFavoritesQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllUserFavoritesQueryResponse
            {
              TotalCount = totalCount,
              userFavorites = userFavoritesDetails  // 🎯 userFavoritesDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "userFavorites başarıyla getirildi.",
            $"userFavoritess.Count userFavorites başarıyla getirildi."  // 🎯 userFavorites sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllUserFavoritesQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "userFavorites getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı favori filtrelerini uygular - Applies user favorites filters
    /// </summary>
    private IQueryable<UserFavoritesViewModel> ApplyUserFavoritesFilters(
        IQueryable<UserFavoritesViewModel> query,
        GetAllUserFavoritesQueryRequest request)
    {
      // 🏢 İşletme bilgileri filtreleri - Business information filters
      if (request.BusinessId.HasValue)
        query = query.Where(x => x.BusinessId == request.BusinessId.Value);

      // ⭐ Favori bilgileri filtreleri - Favorite information filters
      if (!string.IsNullOrEmpty(request.Icon))
        query = query.Where(x => x.Icon != null && x.Icon.Contains(request.Icon));

      return query;
    }
  }
}
