using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.NotificationSettings;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.NotificationSettings.GetAllNotificationSettings
{
  public class GetAllNotificationSettingsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllNotificationSettingsQueryRequest, TransactionResultPack<GetAllNotificationSettingsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllNotificationSettingsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllNotificationSettingsQueryResponse>> Handle(GetAllNotificationSettingsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.notificationSettings
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyNotificationSettingsFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var notificationSettingss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = notificationSettingss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = notificationSettingss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = notificationSettingss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var notificationSettingsDetails = new List<NotificationSettingsDTO>();  // 🎯 notificationSettingsDTO listesi oluştur

        foreach (var notificationSettings in notificationSettingss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (notificationSettings.AuthUserId.HasValue && allUserDetails.ContainsKey(notificationSettings.AuthUserId.Value))
          {
            var userDetail = allUserDetails[notificationSettings.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var notificationSettingsDetail = new NotificationSettingsDTO
          {
            Id = notificationSettings.Id,
            AuthUserId = notificationSettings.AuthUserId,
            AuthCustomerId = notificationSettings.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = notificationSettings.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = notificationSettings.UpdateUserId,
            UpdateUserName = updateUserName,
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

          notificationSettingsDetails.Add(notificationSettingsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllNotificationSettingsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllNotificationSettingsQueryResponse
            {
              TotalCount = totalCount,
              notificationSettings = notificationSettingsDetails  // 🎯 notificationSettingsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "notificationSettings başarıyla getirildi.",
            $"notificationSettingss.Count notificationSettings başarıyla getirildi."  // 🎯 notificationSettings sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllNotificationSettingsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "notificationSettings getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı NotificationSettings filtrelerini uygular - Applies NotificationSettings filters
    /// </summary>
    private IQueryable<NotificationSettingsViewModel> ApplyNotificationSettingsFilters(
        IQueryable<NotificationSettingsViewModel> query,
        GetAllNotificationSettingsQueryRequest request)
    {
      // 🔍 Kullanıcı kimliği filtresi - User ID filter
      if (request.UserId.HasValue)
      {
        query = query.Where(x => x.UserId == request.UserId.Value);
      }

      // 🔍 Ayar türü filtresi - Setting type filter
      if (request.SettingType.HasValue)
      {
        query = query.Where(x => x.SettingType == request.SettingType.Value);
      }

      // 🔍 Etkinlik durumu filtresi - Enabled status filter
      if (request.IsEnabled.HasValue)
      {
        query = query.Where(x => x.IsEnabled == request.IsEnabled.Value);
      }

      // 🔍 Oluşturulma tarihi aralığı filtresi - Creation date range filter
      if (request.CreatedDateFrom.HasValue)
      {
        query = query.Where(x => x.RowCreatedDate >= request.CreatedDateFrom.Value);
      }

      if (request.CreatedDateTo.HasValue)
      {
        query = query.Where(x => x.RowCreatedDate <= request.CreatedDateTo.Value);
      }

      // 🔍 Güncellenme tarihi aralığı filtresi - Update date range filter
      if (request.UpdatedDateFrom.HasValue)
      {
        query = query.Where(x => x.RowUpdatedDate >= request.UpdatedDateFrom.Value);
      }

      if (request.UpdatedDateTo.HasValue)
      {
        query = query.Where(x => x.RowUpdatedDate <= request.UpdatedDateTo.Value);
      }

      return query;
    }
  }
}
