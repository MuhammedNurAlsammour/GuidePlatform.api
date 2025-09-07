using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Businesses;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Businesses.GetAllBusinesses
{
  public class GetAllBusinessesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllBusinessesQueryRequest, TransactionResultPack<GetAllBusinessesQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllBusinessesQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllBusinessesQueryResponse>> Handle(GetAllBusinessesQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.businesses
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyBusinessFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var businessess = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = businessess.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = businessess.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = businessess.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var businessesDetails = new List<BusinessesDTO>();  // 🎯 BusinessesDTO listesi oluştur

        foreach (var businesses in businessess)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (businesses.AuthUserId.HasValue && allUserDetails.ContainsKey(businesses.AuthUserId.Value))
          {
            var userDetail = allUserDetails[businesses.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          businessesDetails.Add(businessesDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllBusinessesQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllBusinessesQueryResponse
            {
              TotalCount = totalCount,
              businesses = businessesDetails  // 🎯 BusinessesDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "Businesses başarıyla getirildi.",
            $"businessess.Count Businesses başarıyla getirildi."  // 🎯 Businesses sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllBusinessesQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "Businesses getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// İşletme filtrelerini uygular - Applies business filters
    /// </summary>
    private IQueryable<BusinessesViewModel> ApplyBusinessFilters(
        IQueryable<BusinessesViewModel> query,
        GetAllBusinessesQueryRequest request)
    {
      // 🏢 Temel iş bilgileri filtreleri - Basic business information filters
      if (!string.IsNullOrEmpty(request.Name))
        query = query.Where(x => x.Name.Contains(request.Name));

      if (!string.IsNullOrEmpty(request.Description))
        query = query.Where(x => x.Description != null && x.Description.Contains(request.Description));

      if (request.CategoryId.HasValue)
        query = query.Where(x => x.CategoryId == request.CategoryId.Value);

      if (request.SubCategoryId.HasValue)
        query = query.Where(x => x.SubCategoryId == request.SubCategoryId.Value);

      // 📍 Konum bilgileri filtreleri - Location information filters
      if (request.ProvinceId.HasValue)
        query = query.Where(x => x.ProvinceId == request.ProvinceId.Value);

      if (request.CountriesId.HasValue)
        query = query.Where(x => x.CountriesId == request.CountriesId.Value);

      if (request.DistrictId.HasValue)
        query = query.Where(x => x.DistrictId == request.DistrictId.Value);

      if (!string.IsNullOrEmpty(request.Address))
        query = query.Where(x => x.Address != null && x.Address.Contains(request.Address));

      // 📞 İletişim bilgileri filtreleri - Contact information filters
      if (!string.IsNullOrEmpty(request.Phone))
        query = query.Where(x => x.Phone != null && x.Phone.Contains(request.Phone));

      if (!string.IsNullOrEmpty(request.Mobile))
        query = query.Where(x => x.Mobile != null && x.Mobile.Contains(request.Mobile));

      if (!string.IsNullOrEmpty(request.Email))
        query = query.Where(x => x.Email != null && x.Email.Contains(request.Email));

      if (!string.IsNullOrEmpty(request.Website))
        query = query.Where(x => x.Website != null && x.Website.Contains(request.Website));

      if (!string.IsNullOrEmpty(request.FacebookUrl))
        query = query.Where(x => x.FacebookUrl != null && x.FacebookUrl.Contains(request.FacebookUrl));

      if (!string.IsNullOrEmpty(request.InstagramUrl))
        query = query.Where(x => x.InstagramUrl != null && x.InstagramUrl.Contains(request.InstagramUrl));

      if (!string.IsNullOrEmpty(request.WhatsApp))
        query = query.Where(x => x.WhatsApp != null && x.WhatsApp.Contains(request.WhatsApp));

      if (!string.IsNullOrEmpty(request.Telegram))
        query = query.Where(x => x.Telegram != null && x.Telegram.Contains(request.Telegram));

      // ⭐ Değerlendirme ve istatistikler filtreleri - Rating and statistics filters
      if (request.MinRating.HasValue)
        query = query.Where(x => x.Rating >= request.MinRating.Value);

      if (request.MaxRating.HasValue)
        query = query.Where(x => x.Rating <= request.MaxRating.Value);

      if (request.MinTotalReviews.HasValue)
        query = query.Where(x => x.TotalReviews >= request.MinTotalReviews.Value);

      if (request.MaxTotalReviews.HasValue)
        query = query.Where(x => x.TotalReviews <= request.MaxTotalReviews.Value);

      if (request.MinViewCount.HasValue)
        query = query.Where(x => x.ViewCount >= request.MinViewCount.Value);

      if (request.MaxViewCount.HasValue)
        query = query.Where(x => x.ViewCount <= request.MaxViewCount.Value);

      // 💼 İş özellikleri filtreleri - Business features filters
      if (request.SubscriptionType.HasValue)
        query = query.Where(x => x.SubscriptionType == request.SubscriptionType.Value);

      if (request.IsVerified.HasValue)
        query = query.Where(x => x.IsVerified == request.IsVerified.Value);

      if (request.IsFeatured.HasValue)
        query = query.Where(x => x.IsFeatured == request.IsFeatured.Value);

      if (!string.IsNullOrEmpty(request.WorkingHours))
        query = query.Where(x => x.WorkingHours != null && x.WorkingHours.Contains(request.WorkingHours));

      if (!string.IsNullOrEmpty(request.Icon))
        query = query.Where(x => x.Icon != null && x.Icon.Contains(request.Icon));

      // 👤 Sahiplik bilgileri filtreleri - Ownership information filters
      if (request.OwnerId.HasValue)
        query = query.Where(x => x.OwnerId == request.OwnerId.Value);

      if (request.AuthUserId.HasValue)
        query = query.Where(x => x.AuthUserId == request.AuthUserId.Value);

      if (request.AuthCustomerId.HasValue)
        query = query.Where(x => x.AuthCustomerId == request.AuthCustomerId.Value);

      // 📅 Tarih filtreleri - Date filters
      if (request.CreatedDateFrom.HasValue)
        query = query.Where(x => x.RowCreatedDate >= request.CreatedDateFrom.Value);

      if (request.CreatedDateTo.HasValue)
        query = query.Where(x => x.RowCreatedDate <= request.CreatedDateTo.Value);

      if (request.UpdatedDateFrom.HasValue)
        query = query.Where(x => x.RowUpdatedDate >= request.UpdatedDateFrom.Value);

      if (request.UpdatedDateTo.HasValue)
        query = query.Where(x => x.RowUpdatedDate <= request.UpdatedDateTo.Value);

      // 🔍 Genel arama - General search
      if (!string.IsNullOrEmpty(request.SearchTerm))
      {
        var searchTerm = request.SearchTerm.ToLower();
        query = query.Where(x =>
          (x.Name != null && x.Name.ToLower().Contains(searchTerm)) ||
          (x.Description != null && x.Description.ToLower().Contains(searchTerm)) ||
          (x.Address != null && x.Address.ToLower().Contains(searchTerm)) ||
          (x.Phone != null && x.Phone.ToLower().Contains(searchTerm)) ||
          (x.Mobile != null && x.Mobile.ToLower().Contains(searchTerm)) ||
          (x.Email != null && x.Email.ToLower().Contains(searchTerm)) ||
          (x.Website != null && x.Website.ToLower().Contains(searchTerm))
        );
      }

      return query;
    }
  }
}
