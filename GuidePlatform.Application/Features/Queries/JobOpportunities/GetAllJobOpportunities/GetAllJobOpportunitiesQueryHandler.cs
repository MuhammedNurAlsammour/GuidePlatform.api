using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.JobOpportunities.GetAllJobOpportunities
{
  public class GetAllJobOpportunitiesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllJobOpportunitiesQueryRequest, TransactionResultPack<GetAllJobOpportunitiesQueryResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly ISharedApiService _sharedApiService;

    public GetAllJobOpportunitiesQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService,
      ISharedApiService sharedApiService) : base(currentUserService, authUserService)
    {
      _context = context;
      _sharedApiService = sharedApiService;
    }

    public async Task<TransactionResultPack<GetAllJobOpportunitiesQueryResponse>> Handle(GetAllJobOpportunitiesQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.jobOpportunities
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Süresi dolmuş iş ilanlarını kontrol et ve deaktive et - Check and deactivate expired job opportunities
        var currentDate = DateTime.UtcNow;

        // Süresi dolmuş aktif kayıtları bul ve deaktive et
        var expiredJobOpportunities = await _context.jobOpportunities
            .Where(x => x.RowIsActive &&
                       !x.RowIsDeleted &&
                       x.RowCreatedDate.AddDays(x.Duration) < currentDate)
            .ToListAsync(cancellationToken);

        if (expiredJobOpportunities.Any())
        {
          foreach (var jobOpportunity in expiredJobOpportunities)
          {
            jobOpportunity.RowIsActive = false;
            jobOpportunity.RowUpdatedDate = currentDate;
          }
          await _context.SaveChangesAsync(cancellationToken);
        }

        // Süresi dolmuş kayıtları filtrele
        baseQuery = baseQuery.Where(x => x.RowCreatedDate.AddDays(x.Duration) >= currentDate);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyJobOpportunitiesFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var jobOpportunitiess = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = jobOpportunitiess.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = jobOpportunitiess.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = jobOpportunitiess.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Business isimlerini toplu olarak al (performans için)
        var businessIds = jobOpportunitiess.Select(b => b.BusinessId).Distinct().ToList();
        var businessNames = await _context.businesses
            .Where(b => businessIds.Contains(b.Id) && b.RowIsActive && !b.RowIsDeleted)
            .Select(b => new { b.Id, b.Name })
            .AsNoTracking()
            .ToDictionaryAsync(b => b.Id, b => b.Name, cancellationToken);

        // 🎯 Province isimlerini Shared API'den toplu olarak al (performans için)
        var provinceIds = jobOpportunitiess.Where(j => j.ProvinceId.HasValue).Select(j => j.ProvinceId!.Value).Distinct().ToList();
        var provinceNames = await _sharedApiService.GetProvincesByIdsAsync(provinceIds, cancellationToken);

        // 🎯 Sponsorlu iş ilanı resimlerini toplu olarak al (ImageType = 13) - Get sponsored job opportunity images
        var sponsoredJobImages = await _context.businessImages
            .Where(bi => businessIds.Contains(bi.BusinessId) &&
                        bi.ImageType == 13 &&
                        bi.RowIsActive && !bi.RowIsDeleted)
            .Select(bi => new
            {
              bi.BusinessId,
              bi.PhotoUrl,
              bi.AltText
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // 🎯 Business ID'ye göre sponsorlu resimleri grupla
        var sponsoredImagesByBusiness = sponsoredJobImages
            .GroupBy(img => img.BusinessId)
            .ToDictionary(
                g => g.Key,
                g => g.FirstOrDefault() // İlk resmi al
            );

        var jobOpportunitiesDetails = new List<JobOpportunitiesDTO>();  // 🎯 jobOpportunitiesDTO listesi oluştur

        foreach (var jobOpportunities in jobOpportunitiess)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (jobOpportunities.AuthUserId.HasValue && allUserDetails.ContainsKey(jobOpportunities.AuthUserId.Value))
          {
            var userDetail = allUserDetails[jobOpportunities.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
          }

          // 🎯 Create/Update kullanıcı bilgilerini al
          string? createUserName = null;
          string? updateUserName = null;

          if (jobOpportunities.CreateUserId.HasValue && allUserDetails.ContainsKey(jobOpportunities.CreateUserId.Value))
          {
            var createUserDetail = allUserDetails[jobOpportunities.CreateUserId.Value];
            createUserName = createUserDetail.AuthUserName;
          }

          if (jobOpportunities.UpdateUserId.HasValue && allUserDetails.ContainsKey(jobOpportunities.UpdateUserId.Value))
          {
            var updateUserDetail = allUserDetails[jobOpportunities.UpdateUserId.Value];
            updateUserName = updateUserDetail.AuthUserName;
          }

          // 🎯 BusinessName ve ProvinceName bilgilerini al
          var businessName = businessNames.GetValueOrDefault(jobOpportunities.BusinessId, "Unknown Business");
          var provinceName = jobOpportunities.ProvinceId.HasValue && provinceNames.ContainsKey(jobOpportunities.ProvinceId.Value)
              ? provinceNames[jobOpportunities.ProvinceId.Value].ProvinceName
              : "Unknown Province";

          // 🎯 Sponsorlu iş ilanı resmi ve metni bilgilerini al
          string? imageJobOpportunitieSponsored = null;
          string? textJobOpportunitieSponsored = null;

          if (sponsoredImagesByBusiness.ContainsKey(jobOpportunities.BusinessId))
          {
            var sponsoredImage = sponsoredImagesByBusiness[jobOpportunities.BusinessId];
            if (sponsoredImage != null)
            {
              imageJobOpportunitieSponsored = sponsoredImage.PhotoUrl;
              textJobOpportunitieSponsored = sponsoredImage.AltText;
            }
          }
          var jobOpportunitiesDetail = new JobOpportunitiesDTO
          {
            Id = jobOpportunities.Id,
            AuthUserId = jobOpportunities.AuthUserId,
            AuthCustomerId = jobOpportunities.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = jobOpportunities.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = jobOpportunities.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = jobOpportunities.RowCreatedDate,
            RowUpdatedDate = jobOpportunities.RowUpdatedDate,
            RowIsActive = jobOpportunities.RowIsActive,
            RowIsDeleted = jobOpportunities.RowIsDeleted,
            BusinessId = jobOpportunities.BusinessId,
            BusinessName = businessName, // Business adı
            Title = jobOpportunities.Title,
            Description = jobOpportunities.Description,
            Phone = jobOpportunities.Phone,
            Duration = jobOpportunities.Duration,
            IsSponsored = jobOpportunities.IsSponsored,
            ProvinceId = jobOpportunities.ProvinceId,
            ProvinceName = provinceName ?? "Unknown Province", // Shared API'den gelen Province adı
            Status = jobOpportunities.Status,
            ImageJobOpportunitieSponsored = imageJobOpportunitieSponsored, // Sponsorlu iş ilanı resmi
            TextJobOpportunitieSponsored = textJobOpportunitieSponsored, // Sponsorlu iş ilanı metni
          };

          jobOpportunitiesDetails.Add(jobOpportunitiesDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllJobOpportunitiesQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllJobOpportunitiesQueryResponse
            {
              TotalCount = totalCount,
              jobOpportunities = jobOpportunitiesDetails  // 🎯 jobOpportunitiesDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "jobOpportunities başarıyla getirildi.",
            $"{jobOpportunitiesDetails.Count} jobOpportunities başarıyla getirildi."  // 🎯 jobOpportunities sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllJobOpportunitiesQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "jobOpportunities getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı JobOpportunities filtrelerini uygular - Applies JobOpportunities filters
    /// </summary>
    private IQueryable<JobOpportunitiesViewModel> ApplyJobOpportunitiesFilters(
        IQueryable<JobOpportunitiesViewModel> query,
        GetAllJobOpportunitiesQueryRequest request)
    {
      // 🎯 İş İlanı Filtreleri - Job Opportunity Filters

      // Business ID filtresi
      if (request.BusinessId.HasValue)
      {
        query = query.Where(x => x.BusinessId == request.BusinessId.Value);
      }

      // Başlık arama filtresi (case-insensitive)
      if (!string.IsNullOrWhiteSpace(request.Title))
      {
        query = query.Where(x => x.Title.ToLower().Contains(request.Title.ToLower()));
      }

      // Açıklama arama filtresi (case-insensitive)
      if (!string.IsNullOrWhiteSpace(request.Description))
      {
        query = query.Where(x => x.Description.ToLower().Contains(request.Description.ToLower()));
      }

      // Telefon numarası arama filtresi
      if (!string.IsNullOrWhiteSpace(request.Phone))
      {
        query = query.Where(x => x.Phone != null && x.Phone.Contains(request.Phone));
      }

      // Süre filtreleri
      if (request.MinDuration.HasValue)
      {
        query = query.Where(x => x.Duration >= request.MinDuration.Value);
      }

      if (request.MaxDuration.HasValue)
      {
        query = query.Where(x => x.Duration <= request.MaxDuration.Value);
      }

      // Sponsorlu ilan filtresi
      if (request.IsSponsored.HasValue)
      {
        query = query.Where(x => x.IsSponsored == request.IsSponsored.Value);
      }

      // Şehir filtresi
      if (request.ProvinceId.HasValue)
      {
        query = query.Where(x => x.ProvinceId == request.ProvinceId.Value);
      }

      // Durum filtresi
      if (request.Status.HasValue)
      {
        query = query.Where(x => x.Status == request.Status.Value);
      }

      // 🎯 Tarih Filtreleri - Date Filters

      // Oluşturulma tarihi filtreleri
      if (request.CreatedDateFrom.HasValue)
      {
        query = query.Where(x => x.RowCreatedDate >= request.CreatedDateFrom.Value);
      }

      if (request.CreatedDateTo.HasValue)
      {
        query = query.Where(x => x.RowCreatedDate <= request.CreatedDateTo.Value);
      }

      // Güncellenme tarihi filtreleri
      if (request.UpdatedDateFrom.HasValue)
      {
        query = query.Where(x => x.RowUpdatedDate >= request.UpdatedDateFrom.Value);
      }

      if (request.UpdatedDateTo.HasValue)
      {
        query = query.Where(x => x.RowUpdatedDate <= request.UpdatedDateTo.Value);
      }

      // 🎯 Kullanıcı Filtreleri - User Filters

      // Auth User ID filtresi
      if (request.AuthUserId.HasValue)
      {
        query = query.Where(x => x.AuthUserId == request.AuthUserId.Value);
      }

      // Auth Customer ID filtresi
      if (request.AuthCustomerId.HasValue)
      {
        query = query.Where(x => x.AuthCustomerId == request.AuthCustomerId.Value);
      }

      // Create User ID filtresi
      if (request.CreateUserId.HasValue)
      {
        query = query.Where(x => x.CreateUserId == request.CreateUserId.Value);
      }

      // Update User ID filtresi
      if (request.UpdateUserId.HasValue)
      {
        query = query.Where(x => x.UpdateUserId == request.UpdateUserId.Value);
      }

      return query;
    }
  }
}
