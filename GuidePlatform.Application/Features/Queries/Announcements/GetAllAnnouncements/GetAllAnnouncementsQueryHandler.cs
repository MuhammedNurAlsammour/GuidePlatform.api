using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Announcements;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Announcements.GetAllAnnouncements
{
  public class GetAllAnnouncementsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllAnnouncementsQueryRequest, TransactionResultPack<GetAllAnnouncementsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllAnnouncementsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllAnnouncementsQueryResponse>> Handle(GetAllAnnouncementsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.announcements
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyAnnouncementsFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var announcementss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = announcementss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = announcementss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = announcementss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var announcementsDetails = new List<AnnouncementsDTO>();  // 🎯 announcementsDTO listesi oluştur

        foreach (var announcements in announcementss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (announcements.AuthUserId.HasValue && allUserDetails.ContainsKey(announcements.AuthUserId.Value))
          {
            var userDetail = allUserDetails[announcements.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var announcementsDetail = new AnnouncementsDTO
          {
            Id = announcements.Id,
            AuthUserId = announcements.AuthUserId,
            AuthCustomerId = announcements.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = announcements.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = announcements.UpdateUserId,
            UpdateUserName = updateUserName,
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

          announcementsDetails.Add(announcementsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllAnnouncementsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllAnnouncementsQueryResponse
            {
              TotalCount = totalCount,
              announcements = announcementsDetails  // 🎯 announcementsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "announcements başarıyla getirildi.",
            $"announcementss.Count announcements başarıyla getirildi."  // 🎯 announcements sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllAnnouncementsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "announcements getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı Announcements filtrelerini uygular - Applies Announcements filters
    /// </summary>
    private IQueryable<AnnouncementsViewModel> ApplyAnnouncementsFilters(
        IQueryable<AnnouncementsViewModel> query,
        GetAllAnnouncementsQueryRequest request)
    {
      // 🔍 Yayın durumu filtresi - Published status filter
      if (request.IsPublished.HasValue)
      {
        query = query.Where(x => x.IsPublished == request.IsPublished.Value);
      }

      // 🔍 Öncelik filtresi - Priority filter
      if (request.Priority.HasValue)
      {
        query = query.Where(x => x.Priority == request.Priority.Value);
      }

      // 🔍 Başlık filtresi - Title filter (içerik arama)
      if (!string.IsNullOrWhiteSpace(request.Title))
      {
        query = query.Where(x => x.Title.Contains(request.Title.Trim()));
      }

      return query;
    }
  }
}
