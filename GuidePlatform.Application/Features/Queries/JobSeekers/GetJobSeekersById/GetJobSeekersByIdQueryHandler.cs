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

namespace GuidePlatform.Application.Features.Queries.JobSeekers.GetJobSeekersById
{
  // Bu handler, bir jobSeekers ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetJobSeekersByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetJobSeekersByIdQueryRequest, TransactionResultPack<GetJobSeekersByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly ISharedApiService _sharedApiService;

    public GetJobSeekersByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService,
        ISharedApiService sharedApiService) : base(currentUserService, authUserService)
    {
      _context = context;
      _sharedApiService = sharedApiService;
    }

    public async Task<TransactionResultPack<GetJobSeekersByIdQueryResponse>> Handle(GetJobSeekersByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetJobSeekersByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "jobSeekers ID'si belirtilmedi.",
              "jobSeekers ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var jobSeekersId = request.GetIdAsGuid();
        if (!jobSeekersId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetJobSeekersByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz jobSeekers ID formatı.",
              $"Geçersiz jobSeekers ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
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

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == jobSeekersId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve jobSeekers çekiliyor
        var jobSeekers = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == jobSeekersId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (jobSeekers == null)
        {
          // jobSeekers bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetJobSeekersByIdQueryResponse>(
              request.Id,
              null,
              "Hata / jobSeekers Bulunamadı",
              "Belirtilen ID'ye sahip jobSeekers bulunamadı.",
              $"ID '{request.Id}' ile eşleşen jobSeekers bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (jobSeekers.AuthUserId.HasValue)
          allUserIds.Add(jobSeekers.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (jobSeekers.CreateUserId.HasValue)
          allUserIds.Add(jobSeekers.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (jobSeekers.UpdateUserId.HasValue)
          allUserIds.Add(jobSeekers.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (jobSeekers.AuthUserId.HasValue && allUserDetails.ContainsKey(jobSeekers.AuthUserId.Value))
        {
          var userDetail = allUserDetails[jobSeekers.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Business isimlerini toplu olarak al (performans için)
        var businessIds = new List<Guid> { jobSeekers.BusinessId };
        var businessNames = await _context.businesses
            .Where(b => businessIds.Contains(b.Id) && b.RowIsActive && !b.RowIsDeleted)
            .Select(b => new { b.Id, b.Name })
            .AsNoTracking()
            .ToDictionaryAsync(b => b.Id, b => b.Name, cancellationToken);

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

        // 🎯 BusinessName bilgilerini al
        var businessName = businessNames.GetValueOrDefault(jobSeekers.BusinessId, "Unknown Business");

        // 🎯 Province bilgisini Shared API'den al
        string? provinceName = null;
        if (jobSeekers.ProvinceId.HasValue)
        {
          var province = await _sharedApiService.GetProvinceByIdAsync(jobSeekers.ProvinceId.Value, cancellationToken);
          provinceName = province?.ProvinceName ?? "Unknown Province";
        }

        // 🎯 Sponsorlu iş arayan resimlerini al (ImageType = 12) - Get sponsored job seeker images
        string? imageJobSeekerSponsored = null;
        string? textJobSeekerSponsored = null;

        var sponsoredJobSeekerImage = await _context.businessImages
            .Where(bi => bi.BusinessId == jobSeekers.BusinessId &&
                        bi.ImageType == 12 &&
                        bi.RowIsActive && !bi.RowIsDeleted)
            .Select(bi => new
            {
              bi.PhotoUrl,
              bi.AltText
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (sponsoredJobSeekerImage != null)
        {
          imageJobSeekerSponsored = sponsoredJobSeekerImage.PhotoUrl;
          textJobSeekerSponsored = sponsoredJobSeekerImage.AltText;
        }

        // jobSeekers detay DTO'su oluşturuluyor
        var jobSeekersDetail = new JobSeekersDTO
        {
          Id = jobSeekers.Id,
          AuthUserId = jobSeekers.AuthUserId,
          AuthCustomerId = jobSeekers.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = jobSeekers.CreateUserId,
          UpdateUserId = jobSeekers.UpdateUserId,
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

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetJobSeekersByIdQueryResponse>(
            new GetJobSeekersByIdQueryResponse
            {
              jobSeekers = jobSeekersDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "jobSeekers başarıyla getirildi.",
            $"jobSeekers Id: {jobSeekers.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetJobSeekersByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "jobSeekers getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

