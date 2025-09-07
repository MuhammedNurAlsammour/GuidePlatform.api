using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.NotificationSettings;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.NotificationSettings.GetNotificationSettingsById
{
  // Bu handler, bir notificationSettings ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetNotificationSettingsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetNotificationSettingsByIdQueryRequest, TransactionResultPack<GetNotificationSettingsByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetNotificationSettingsByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetNotificationSettingsByIdQueryResponse>> Handle(GetNotificationSettingsByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetNotificationSettingsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "notificationSettings ID'si belirtilmedi.",
              "notificationSettings ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var notificationSettingsId = request.GetIdAsGuid();
        if (!notificationSettingsId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetNotificationSettingsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz notificationSettings ID formatı.",
              $"Geçersiz notificationSettings ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.notificationSettings
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == notificationSettingsId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve notificationSettings çekiliyor
        var notificationSettings = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == notificationSettingsId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (notificationSettings == null)
        {
          // notificationSettings bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetNotificationSettingsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / notificationSettings Bulunamadı",
              "Belirtilen ID'ye sahip notificationSettings bulunamadı.",
              $"ID '{request.Id}' ile eşleşen notificationSettings bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (notificationSettings.AuthUserId.HasValue)
          allUserIds.Add(notificationSettings.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (notificationSettings.CreateUserId.HasValue)
          allUserIds.Add(notificationSettings.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (notificationSettings.UpdateUserId.HasValue)
          allUserIds.Add(notificationSettings.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (notificationSettings.AuthUserId.HasValue && allUserDetails.ContainsKey(notificationSettings.AuthUserId.Value))
        {
          var userDetail = allUserDetails[notificationSettings.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (notificationSettings.CreateUserId.HasValue && allUserDetails.ContainsKey(notificationSettings.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[notificationSettings.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (notificationSettings.UpdateUserId.HasValue && allUserDetails.ContainsKey(notificationSettings.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[notificationSettings.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // notificationSettings detay DTO'su oluşturuluyor
        var notificationSettingsDetail = new NotificationSettingsDTO
        {
          Id = notificationSettings.Id,
          AuthUserId = notificationSettings.AuthUserId,
          AuthCustomerId = notificationSettings.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = notificationSettings.CreateUserId,
          UpdateUserId = notificationSettings.UpdateUserId,
          RowCreatedDate = notificationSettings.RowCreatedDate,
          RowUpdatedDate = notificationSettings.RowUpdatedDate,
          RowIsActive = notificationSettings.RowIsActive,
          RowIsDeleted = notificationSettings.RowIsDeleted,
          // Bildirim ayarları özel alanları - Notification settings specific fields
          UserId = notificationSettings.UserId,
          SettingType = notificationSettings.SettingType,
          IsEnabled = notificationSettings.IsEnabled,
          Icon = notificationSettings.Icon
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetNotificationSettingsByIdQueryResponse>(
            new GetNotificationSettingsByIdQueryResponse
            {
              notificationSettings = notificationSettingsDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "notificationSettings başarıyla getirildi.",
            $"notificationSettings Id: {notificationSettings.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetNotificationSettingsByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "notificationSettings getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

