using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessWorkingHours;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetBusinessWorkingHoursById
{
  // Bu handler, bir businessWorkingHours ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetBusinessWorkingHoursByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBusinessWorkingHoursByIdQueryRequest, TransactionResultPack<GetBusinessWorkingHoursByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetBusinessWorkingHoursByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetBusinessWorkingHoursByIdQueryResponse>> Handle(GetBusinessWorkingHoursByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessWorkingHoursByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "businessWorkingHours ID'si belirtilmedi.",
              "businessWorkingHours ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var businessWorkingHoursId = request.GetIdAsGuid();
        if (!businessWorkingHoursId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessWorkingHoursByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz businessWorkingHours ID formatı.",
              $"Geçersiz businessWorkingHours ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.businessWorkingHours
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // BusinessId filtresi ekle (eğer verilmişse)
        if (!string.IsNullOrWhiteSpace(request.BusinessId))
        {
          var businessId = Guid.Parse(request.BusinessId);
          baseQuery = baseQuery.Where(x => x.BusinessId == businessId);
        }

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == businessWorkingHoursId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve businessWorkingHours çekiliyor
        var businessWorkingHours = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == businessWorkingHoursId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (businessWorkingHours == null)
        {
          // businessWorkingHours bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessWorkingHoursByIdQueryResponse>(
              request.Id,
              null,
              "Hata / businessWorkingHours Bulunamadı",
              "Belirtilen ID'ye sahip businessWorkingHours bulunamadı.",
              $"ID '{request.Id}' ile eşleşen businessWorkingHours bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (businessWorkingHours.AuthUserId.HasValue)
          allUserIds.Add(businessWorkingHours.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (businessWorkingHours.CreateUserId.HasValue)
          allUserIds.Add(businessWorkingHours.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (businessWorkingHours.UpdateUserId.HasValue)
          allUserIds.Add(businessWorkingHours.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (businessWorkingHours.AuthUserId.HasValue && allUserDetails.ContainsKey(businessWorkingHours.AuthUserId.Value))
        {
          var userDetail = allUserDetails[businessWorkingHours.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (businessWorkingHours.CreateUserId.HasValue && allUserDetails.ContainsKey(businessWorkingHours.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[businessWorkingHours.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (businessWorkingHours.UpdateUserId.HasValue && allUserDetails.ContainsKey(businessWorkingHours.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[businessWorkingHours.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // businessWorkingHours detay DTO'su oluşturuluyor
        var businessWorkingHoursDetail = new BusinessWorkingHoursDTO
        {
          Id = businessWorkingHours.Id,
          BusinessId = businessWorkingHours.BusinessId,
          DayOfWeek = businessWorkingHours.DayOfWeek,
          OpenTime = businessWorkingHours.OpenTime,
          CloseTime = businessWorkingHours.CloseTime,
          IsClosed = businessWorkingHours.IsClosed,
          Icon = businessWorkingHours.Icon,
          AuthUserId = businessWorkingHours.AuthUserId,
          AuthCustomerId = businessWorkingHours.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = businessWorkingHours.CreateUserId,
          UpdateUserId = businessWorkingHours.UpdateUserId,
          RowCreatedDate = businessWorkingHours.RowCreatedDate,
          RowUpdatedDate = businessWorkingHours.RowUpdatedDate,
          RowIsActive = businessWorkingHours.RowIsActive,
          RowIsDeleted = businessWorkingHours.RowIsDeleted
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetBusinessWorkingHoursByIdQueryResponse>(
            new GetBusinessWorkingHoursByIdQueryResponse
            {
              businessWorkingHours = businessWorkingHoursDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "businessWorkingHours başarıyla getirildi.",
            $"businessWorkingHours Id: {businessWorkingHours.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetBusinessWorkingHoursByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "businessWorkingHours getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

