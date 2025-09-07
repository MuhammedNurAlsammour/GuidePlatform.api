using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Notifications;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Notifications.GetAllNotifications
{
  public class GetAllNotificationsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllNotificationsQueryRequest, TransactionResultPack<GetAllNotificationsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllNotificationsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllNotificationsQueryResponse>> Handle(GetAllNotificationsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.notifications
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyNotificationsFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var notificationss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = notificationss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = notificationss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = notificationss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var notificationsDetails = new List<NotificationsDTO>();  // 🎯 notificationsDTO listesi oluştur

        foreach (var notifications in notificationss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (notifications.AuthUserId.HasValue && allUserDetails.ContainsKey(notifications.AuthUserId.Value))
          {
            var userDetail = allUserDetails[notifications.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var notificationsDetail = new NotificationsDTO
          {
            Id = notifications.Id,
            AuthUserId = notifications.AuthUserId,
            AuthCustomerId = notifications.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = notifications.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = notifications.UpdateUserId,
            UpdateUserName = updateUserName,
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

          notificationsDetails.Add(notificationsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllNotificationsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllNotificationsQueryResponse
            {
              TotalCount = totalCount,
              notifications = notificationsDetails  // 🎯 notificationsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "notifications başarıyla getirildi.",
            $"notificationss.Count notifications başarıyla getirildi."  // 🎯 notifications sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllNotificationsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "notifications getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı Notifications filtrelerini uygular - Applies Notifications filters
    /// </summary>
    private IQueryable<NotificationsViewModel> ApplyNotificationsFilters(
        IQueryable<NotificationsViewModel> query,
        GetAllNotificationsQueryRequest request)
    {
      // 🔍 Alıcı kullanıcı kimliği filtresi - Recipient user ID filter
      if (request.RecipientUserId.HasValue)
      {
        query = query.Where(x => x.RecipientUserId == request.RecipientUserId.Value);
      }

      // 🔍 Başlık filtresi - Title filter (içerik arama)
      if (!string.IsNullOrWhiteSpace(request.Title))
      {
        query = query.Where(x => x.Title.Contains(request.Title.Trim()));
      }

      // 🔍 Mesaj filtresi - Message filter (içerik arama)
      if (!string.IsNullOrWhiteSpace(request.Message))
      {
        query = query.Where(x => x.Message.Contains(request.Message.Trim()));
      }

      // 🔍 Bildirim türü filtresi - Notification type filter
      if (!string.IsNullOrWhiteSpace(request.NotificationType))
      {
        query = query.Where(x => x.NotificationType == request.NotificationType.Trim());
      }

      // 🔍 Okunma durumu filtresi - Read status filter
      if (request.IsRead.HasValue)
      {
        query = query.Where(x => x.IsRead == request.IsRead.Value);
      }

      // 🔍 Okuma tarihi aralığı filtresi - Read date range filter
      if (request.ReadDateFrom.HasValue)
      {
        query = query.Where(x => x.ReadDate.HasValue && x.ReadDate.Value >= request.ReadDateFrom.Value);
      }

      if (request.ReadDateTo.HasValue)
      {
        query = query.Where(x => x.ReadDate.HasValue && x.ReadDate.Value <= request.ReadDateTo.Value);
      }

      // 🔍 İşlem URL'si filtresi - Action URL filter
      if (!string.IsNullOrWhiteSpace(request.ActionUrl))
      {
        query = query.Where(x => x.ActionUrl != null && x.ActionUrl.Contains(request.ActionUrl.Trim()));
      }

      // 🔍 İlişkili varlık kimliği filtresi - Related entity ID filter
      if (request.RelatedEntityId.HasValue)
      {
        query = query.Where(x => x.RelatedEntityId == request.RelatedEntityId.Value);
      }

      // 🔍 İlişkili varlık türü filtresi - Related entity type filter
      if (!string.IsNullOrWhiteSpace(request.RelatedEntityType))
      {
        query = query.Where(x => x.RelatedEntityType == request.RelatedEntityType.Trim());
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

      return query;
    }
  }
}
