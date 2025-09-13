using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Announcements;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Announcements.GetAnnouncementsById
{
  // Bu handler, bir announcements ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetAnnouncementsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetAnnouncementsByIdQueryRequest, TransactionResultPack<GetAnnouncementsByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAnnouncementsByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAnnouncementsByIdQueryResponse>> Handle(GetAnnouncementsByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetAnnouncementsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "announcements ID'si belirtilmedi.",
              "announcements ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var announcementsId = request.GetIdAsGuid();
        if (!announcementsId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetAnnouncementsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz announcements ID formatı.",
              $"Geçersiz announcements ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.announcements
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == announcementsId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve announcements çekiliyor
        var announcements = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == announcementsId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (announcements == null)
        {
          // announcements bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetAnnouncementsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / announcements Bulunamadı",
              "Belirtilen ID'ye sahip announcements bulunamadı.",
              $"ID '{request.Id}' ile eşleşen announcements bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (announcements.AuthUserId.HasValue)
          allUserIds.Add(announcements.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (announcements.CreateUserId.HasValue)
          allUserIds.Add(announcements.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (announcements.UpdateUserId.HasValue)
          allUserIds.Add(announcements.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (announcements.AuthUserId.HasValue && allUserDetails.ContainsKey(announcements.AuthUserId.Value))
        {
          var userDetail = allUserDetails[announcements.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (announcements.CreateUserId.HasValue && allUserDetails.ContainsKey(announcements.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[announcements.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (announcements.UpdateUserId.HasValue && allUserDetails.ContainsKey(announcements.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[announcements.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // announcements detay DTO'su oluşturuluyor
        var announcementsDetail = new AnnouncementsDTO
        {
          Id = announcements.Id,
          AuthUserId = announcements.AuthUserId,
          AuthCustomerId = announcements.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = announcements.CreateUserId,
          UpdateUserId = announcements.UpdateUserId,
          RowCreatedDate = announcements.RowCreatedDate,
          RowUpdatedDate = announcements.RowUpdatedDate,
          RowIsActive = announcements.RowIsActive,
          RowIsDeleted = announcements.RowIsDeleted,
          // Duyuru özel alanları - Announcement specific fields
          Title = announcements.Title,
          Content = announcements.Content,
          Priority = announcements.Priority,
          IsPublished = announcements.IsPublished,
          PublishedDate = announcements.PublishedDate,
          Icon = announcements.Icon
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetAnnouncementsByIdQueryResponse>(
            new GetAnnouncementsByIdQueryResponse
            {
              announcements = announcementsDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "announcements başarıyla getirildi.",
            $"announcements Id: {announcements.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetAnnouncementsByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "announcements getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

