using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Notifications;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Notifications.GetNotificationsById
{
  // Bu handler, bir notifications ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetNotificationsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetNotificationsByIdQueryRequest, TransactionResultPack<GetNotificationsByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetNotificationsByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetNotificationsByIdQueryResponse>> Handle(GetNotificationsByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetNotificationsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "notifications ID'si belirtilmedi.",
              "notifications ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var notificationsId = request.GetIdAsGuid();
        if (!notificationsId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetNotificationsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz notifications ID formatı.",
              $"Geçersiz notifications ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.notifications
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == notificationsId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve notifications çekiliyor
        var notifications = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == notificationsId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (notifications == null)
        {
          // notifications bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetNotificationsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / notifications Bulunamadı",
              "Belirtilen ID'ye sahip notifications bulunamadı.",
              $"ID '{request.Id}' ile eşleşen notifications bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (notifications.AuthUserId.HasValue)
          allUserIds.Add(notifications.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (notifications.CreateUserId.HasValue)
          allUserIds.Add(notifications.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (notifications.UpdateUserId.HasValue)
          allUserIds.Add(notifications.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (notifications.AuthUserId.HasValue && allUserDetails.ContainsKey(notifications.AuthUserId.Value))
        {
          var userDetail = allUserDetails[notifications.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (notifications.CreateUserId.HasValue && allUserDetails.ContainsKey(notifications.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[notifications.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (notifications.UpdateUserId.HasValue && allUserDetails.ContainsKey(notifications.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[notifications.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // notifications detay DTO'su oluşturuluyor
        var notificationsDetail = new NotificationsDTO
        {
          Id = notifications.Id,
          AuthUserId = notifications.AuthUserId,
          AuthCustomerId = notifications.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = notifications.CreateUserId,
          UpdateUserId = notifications.UpdateUserId,
          RowCreatedDate = notifications.RowCreatedDate,
          RowUpdatedDate = notifications.RowUpdatedDate,
          RowIsActive = notifications.RowIsActive,
          RowIsDeleted = notifications.RowIsDeleted,
          // Bildirim özel alanları - Notification specific fields
          RecipientUserId = notifications.RecipientUserId,
          Title = notifications.Title,
          Message = notifications.Message,
          NotificationType = notifications.NotificationType,
          IsRead = notifications.IsRead,
          ReadDate = notifications.ReadDate,
          ActionUrl = notifications.ActionUrl,
          RelatedEntityId = notifications.RelatedEntityId,
          RelatedEntityType = notifications.RelatedEntityType,
          Icon = notifications.Icon
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetNotificationsByIdQueryResponse>(
            new GetNotificationsByIdQueryResponse
            {
              notifications = notificationsDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "notifications başarıyla getirildi.",
            $"notifications Id: {notifications.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetNotificationsByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "notifications getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

