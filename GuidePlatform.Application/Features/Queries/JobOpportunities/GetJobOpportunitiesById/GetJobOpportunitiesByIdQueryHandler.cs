using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.JobOpportunities.GetJobOpportunitiesById
{
  // Bu handler, bir jobOpportunities ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetJobOpportunitiesByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetJobOpportunitiesByIdQueryRequest, TransactionResultPack<GetJobOpportunitiesByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly ISharedApiService _sharedApiService;

    public GetJobOpportunitiesByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService,
        ISharedApiService sharedApiService) : base(currentUserService, authUserService)
    {
      _context = context;
      _sharedApiService = sharedApiService;
    }

    public async Task<TransactionResultPack<GetJobOpportunitiesByIdQueryResponse>> Handle(GetJobOpportunitiesByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetJobOpportunitiesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "jobOpportunities ID'si belirtilmedi.",
              "jobOpportunities ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var jobOpportunitiesId = request.GetIdAsGuid();
        if (!jobOpportunitiesId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetJobOpportunitiesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz jobOpportunities ID formatı.",
              $"Geçersiz jobOpportunities ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
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

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == jobOpportunitiesId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve jobOpportunities çekiliyor
        var jobOpportunities = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == jobOpportunitiesId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (jobOpportunities == null)
        {
          // jobOpportunities bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetJobOpportunitiesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / jobOpportunities Bulunamadı",
              "Belirtilen ID'ye sahip jobOpportunities bulunamadı.",
              $"ID '{request.Id}' ile eşleşen jobOpportunities bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (jobOpportunities.AuthUserId.HasValue)
          allUserIds.Add(jobOpportunities.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (jobOpportunities.CreateUserId.HasValue)
          allUserIds.Add(jobOpportunities.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (jobOpportunities.UpdateUserId.HasValue)
          allUserIds.Add(jobOpportunities.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (jobOpportunities.AuthUserId.HasValue && allUserDetails.ContainsKey(jobOpportunities.AuthUserId.Value))
        {
          var userDetail = allUserDetails[jobOpportunities.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
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

        // 🎯 Province bilgisini Shared API'den al
        string? provinceName = null;
        if (jobOpportunities.ProvinceId.HasValue)
        {
          var province = await _sharedApiService.GetProvinceByIdAsync(jobOpportunities.ProvinceId.Value, cancellationToken);
          provinceName = province?.ProvinceName ?? "Unknown Province";
        }

        // 🎯 Sponsorlu iş ilanı resimlerini al (ImageType = 13) - Get sponsored job opportunity images
        string? imageJobOpportunitieSponsored = null;
        string? textJobOpportunitieSponsored = null;

        var sponsoredJobImage = await _context.businessImages
            .Where(bi => bi.BusinessId == jobOpportunities.BusinessId &&
                        bi.ImageType == 13 &&
                        bi.RowIsActive && !bi.RowIsDeleted)
            .Select(bi => new
            {
              bi.PhotoUrl,
              bi.AltText
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (sponsoredJobImage != null)
        {
          imageJobOpportunitieSponsored = sponsoredJobImage.PhotoUrl;
          textJobOpportunitieSponsored = sponsoredJobImage.AltText;
        }

        // jobOpportunities detay DTO'su oluşturuluyor
        var jobOpportunitiesDetail = new JobOpportunitiesDTO
        {
          Id = jobOpportunities.Id,
          AuthUserId = jobOpportunities.AuthUserId,
          AuthCustomerId = jobOpportunities.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = jobOpportunities.CreateUserId,
          UpdateUserId = jobOpportunities.UpdateUserId,
          RowCreatedDate = jobOpportunities.RowCreatedDate,
          RowUpdatedDate = jobOpportunities.RowUpdatedDate,
          RowIsActive = jobOpportunities.RowIsActive,
          RowIsDeleted = jobOpportunities.RowIsDeleted,
          BusinessId = jobOpportunities.BusinessId,
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

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetJobOpportunitiesByIdQueryResponse>(
            new GetJobOpportunitiesByIdQueryResponse
            {
              jobOpportunities = jobOpportunitiesDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "jobOpportunities başarıyla getirildi.",
            $"jobOpportunities Id: {jobOpportunities.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetJobOpportunitiesByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "jobOpportunities getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

