using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Businesses;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

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
  }
}
