using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.JobSeekers.GetAllJobSeekers
{
  public class GetAllJobSeekersQueryHandler : BaseQueryHandler, IRequestHandler<GetAllJobSeekersQueryRequest, TransactionResultPack<GetAllJobSeekersQueryResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly ISharedApiService _sharedApiService;

    public GetAllJobSeekersQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService,
      ISharedApiService sharedApiService) : base(currentUserService, authUserService)
    {
      _context = context;
      _sharedApiService = sharedApiService;
    }

    public async Task<TransactionResultPack<GetAllJobSeekersQueryResponse>> Handle(GetAllJobSeekersQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.jobSeekers
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Süresi dolmuş iş arayanları kontrol et ve deaktive et - Check and deactivate expired job seekers
        var currentDate = DateTime.UtcNow;

        // Süresi dolmuş aktif kayıtları bul ve deaktive et
        var expiredJobSeekers = await _context.jobSeekers
            .Where(x => x.RowIsActive &&
                       !x.RowIsDeleted &&
                       x.RowCreatedDate.AddDays(x.Duration) < currentDate)
            .ToListAsync(cancellationToken);

        if (expiredJobSeekers.Any())
        {
          foreach (var jobSeeker in expiredJobSeekers)
          {
            jobSeeker.RowIsActive = false;
            jobSeeker.RowUpdatedDate = currentDate;
          }
          await _context.SaveChangesAsync(cancellationToken);
        }

        // Süresi dolmuş kayıtları filtrele
        baseQuery = baseQuery.Where(x => x.RowCreatedDate.AddDays(x.Duration) >= currentDate);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyJobSeekersFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var jobSeekerss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = jobSeekerss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = jobSeekerss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = jobSeekerss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Business isimlerini toplu olarak al (performans için)
        var businessIds = jobSeekerss.Select(b => b.BusinessId).Distinct().ToList();
        var businessNames = await _context.businesses
            .Where(b => businessIds.Contains(b.Id) && b.RowIsActive && !b.RowIsDeleted)
            .Select(b => new { b.Id, b.Name })
            .AsNoTracking()
            .ToDictionaryAsync(b => b.Id, b => b.Name, cancellationToken);

        // 🎯 Province isimlerini Shared API'den toplu olarak al (performans için)
        var provinceIds = jobSeekerss.Where(j => j.ProvinceId.HasValue).Select(j => j.ProvinceId!.Value).Distinct().ToList();
        var provinceNames = await _sharedApiService.GetProvincesByIdsAsync(provinceIds, cancellationToken);

        // 🎯 Sponsorlu iş arayan resimlerini toplu olarak al (ImageType = 12) - Get sponsored job seeker images
        var sponsoredJobSeekerImages = await _context.businessImages
            .Where(bi => businessIds.Contains(bi.BusinessId) &&
                        bi.ImageType == 12 &&
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
        var sponsoredImagesByBusiness = sponsoredJobSeekerImages
            .GroupBy(img => img.BusinessId)
            .ToDictionary(
                g => g.Key,
                g => g.FirstOrDefault() // İlk resmi al
            );

        var jobSeekersDetails = new List<JobSeekersDTO>();  // 🎯 jobSeekersDTO listesi oluştur

        foreach (var jobSeekers in jobSeekerss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (jobSeekers.AuthUserId.HasValue && allUserDetails.ContainsKey(jobSeekers.AuthUserId.Value))
          {
            var userDetail = allUserDetails[jobSeekers.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
          }


          // 🎯 Create/Update kullanıcı bilgilerini al
          string? createUserName = null;
          string? updateUserName = null;

          if (jobSeekers.CreateUserId.HasValue && allUserDetails.ContainsKey(jobSeekers.CreateUserId.Value))
          {
            var createUserDetail = allUserDetails[jobSeekers.CreateUserId.Value];
            createUserName = createUserDetail.AuthUserName;
          }

          if (jobSeekers.UpdateUserId.HasValue && allUserDetails.ContainsKey(jobSeekers.UpdateUserId.Value))
          {
            var updateUserDetail = allUserDetails[jobSeekers.UpdateUserId.Value];
            updateUserName = updateUserDetail.AuthUserName;
          }

          // 🎯 BusinessName ve ProvinceName bilgilerini al
          var businessName = businessNames.GetValueOrDefault(jobSeekers.BusinessId, "Unknown Business");
          var provinceName = jobSeekers.ProvinceId.HasValue && provinceNames.ContainsKey(jobSeekers.ProvinceId.Value)
              ? provinceNames[jobSeekers.ProvinceId.Value].ProvinceName
              : "Unknown Province";

          // 🎯 Sponsorlu iş arayan resmi ve metni bilgilerini al
          string? imageJobSeekerSponsored = null;
          string? textJobSeekerSponsored = null;

          if (sponsoredImagesByBusiness.ContainsKey(jobSeekers.BusinessId))
          {
            var sponsoredImage = sponsoredImagesByBusiness[jobSeekers.BusinessId];
            if (sponsoredImage != null)
            {
              imageJobSeekerSponsored = sponsoredImage.PhotoUrl;
              textJobSeekerSponsored = sponsoredImage.AltText;
            }
          }

          var jobSeekersDetail = new JobSeekersDTO
          {
            Id = jobSeekers.Id,
            AuthUserId = jobSeekers.AuthUserId,
            AuthCustomerId = jobSeekers.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = jobSeekers.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = jobSeekers.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = jobSeekers.RowCreatedDate,
            RowUpdatedDate = jobSeekers.RowUpdatedDate,
            RowIsActive = jobSeekers.RowIsActive,
            RowIsDeleted = jobSeekers.RowIsDeleted,
            BusinessId = jobSeekers.BusinessId,
            BusinessName = businessName, // Business adı
            FullName = jobSeekers.FullName,
            Description = jobSeekers.Description,
            Phone = jobSeekers.Phone,
            Duration = jobSeekers.Duration,
            IsSponsored = jobSeekers.IsSponsored,
            ProvinceId = jobSeekers.ProvinceId,
            ProvinceName = provinceName ?? "Unknown Province", // Shared API'den gelen Province adı
            Status = jobSeekers.Status,
            ImageJobSeekerSponsored = imageJobSeekerSponsored, // Sponsorlu iş arayan resmi
            TextJobSeekerSponsored = textJobSeekerSponsored, // Sponsorlu iş arayan metni
          };

          jobSeekersDetails.Add(jobSeekersDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllJobSeekersQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllJobSeekersQueryResponse
            {
              TotalCount = totalCount,
              jobSeekers = jobSeekersDetails  // 🎯 jobSeekersDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "jobSeekers başarıyla getirildi.",
            $"{jobSeekersDetails.Count} jobSeekers başarıyla getirildi."  // 🎯 jobSeekers sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllJobSeekersQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "jobSeekers getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı JobSeekers filtrelerini uygular - Applies JobSeekers filters
    /// </summary>
    private IQueryable<JobSeekersViewModel> ApplyJobSeekersFilters(
        IQueryable<JobSeekersViewModel> query,
        GetAllJobSeekersQueryRequest request)
    {
      // 🎯 İş Arayan Filtreleri - Job Seeker Filters

      // Business ID filtresi
      if (request.BusinessId.HasValue)
      {
        query = query.Where(x => x.BusinessId == request.BusinessId.Value);
      }

      // Tam isim arama filtresi (case-insensitive)
      if (!string.IsNullOrWhiteSpace(request.FullName))
      {
        query = query.Where(x => x.FullName.ToLower().Contains(request.FullName.ToLower()));
      }

      // Açıklama arama filtresi (case-insensitive)
      if (!string.IsNullOrWhiteSpace(request.Description))
      {
        query = query.Where(x => x.Description != null && x.Description.ToLower().Contains(request.Description.ToLower()));
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

      // Sponsorlu arayan filtresi
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
