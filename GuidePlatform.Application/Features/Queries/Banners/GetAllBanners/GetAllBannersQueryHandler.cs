using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Banners;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Banners.GetAllBanners
{
  public class GetAllBannersQueryHandler : BaseQueryHandler, IRequestHandler<GetAllBannersQueryRequest, TransactionResultPack<GetAllBannersQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllBannersQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllBannersQueryResponse>> Handle(GetAllBannersQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.banners
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyBannersFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var bannerss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = bannerss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = bannerss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = bannerss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var bannersDetails = new List<BannersDTO>();  // 🎯 bannersDTO listesi oluştur

        foreach (var banners in bannerss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (banners.AuthUserId.HasValue && allUserDetails.ContainsKey(banners.AuthUserId.Value))
          {
            var userDetail = allUserDetails[banners.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var bannersDetail = new BannersDTO
          {
            Id = banners.Id,
            AuthUserId = banners.AuthUserId,
            AuthCustomerId = banners.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = banners.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = banners.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = banners.RowCreatedDate,
            RowUpdatedDate = banners.RowUpdatedDate,
            RowIsActive = banners.RowIsActive,
            RowIsDeleted = banners.RowIsDeleted,
            // Banner özel alanları - Banner specific fields
            Title = banners.Title,
            Description = banners.Description,
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
            ProvinceId = banners.ProvinceId,
            Icon = banners.Icon
          };

          bannersDetails.Add(bannersDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllBannersQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllBannersQueryResponse
            {
              TotalCount = totalCount,
              banners = bannersDetails  // 🎯 bannersDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "banners başarıyla getirildi.",
            $"bannerss.Count banners başarıyla getirildi."  // 🎯 banners sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllBannersQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "banners getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı Banners filtrelerini uygular - Applies Banners filters
    /// </summary>
    private IQueryable<BannersViewModel> ApplyBannersFilters(
        IQueryable<BannersViewModel> query,
        GetAllBannersQueryRequest request)
    {
      // 🔍 Aktif durumu filtresi - Active status filter
      if (request.IsActive.HasValue)
      {
        query = query.Where(x => x.IsActive == request.IsActive.Value);
      }

      // 🔍 il durumu filtresi - Province status filter
      if (request.ProvinceId.HasValue)
      {
        query = query.Where(x => x.ProvinceId == request.ProvinceId.Value);
      }

      // 🔍 Başlık filtresi - Title filter (içerik arama)
      if (!string.IsNullOrWhiteSpace(request.Title))
      {
        query = query.Where(x => x.Title.Contains(request.Title.Trim()));
      }

      // 🔍 Başlangıç tarihi filtresi - Start date filter
      if (request.StartDateFrom.HasValue)
      {
        query = query.Where(x => x.StartDate >= request.StartDateFrom.Value);
      }

      if (request.StartDateTo.HasValue)
      {
        query = query.Where(x => x.StartDate <= request.StartDateTo.Value);
      }

      // 🔍 Bitiş tarihi filtresi - End date filter
      if (request.EndDateFrom.HasValue)
      {
        query = query.Where(x => x.EndDate.HasValue && x.EndDate.Value >= request.EndDateFrom.Value);
      }

      if (request.EndDateTo.HasValue)
      {
        query = query.Where(x => x.EndDate.HasValue && x.EndDate.Value <= request.EndDateTo.Value);
      }

      return query;
    }
  }
}
