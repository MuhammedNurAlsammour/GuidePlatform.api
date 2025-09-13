using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.SearchLogs;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.SearchLogs.GetSearchLogsById
{
  // Bu handler, bir searchLogs ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetSearchLogsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetSearchLogsByIdQueryRequest, TransactionResultPack<GetSearchLogsByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetSearchLogsByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetSearchLogsByIdQueryResponse>> Handle(GetSearchLogsByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetSearchLogsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "searchLogs ID'si belirtilmedi.",
              "searchLogs ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var searchLogsId = request.GetIdAsGuid();
        if (!searchLogsId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetSearchLogsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz searchLogs ID formatı.",
              $"Geçersiz searchLogs ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.searchLogs
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == searchLogsId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve searchLogs çekiliyor
        var searchLogs = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == searchLogsId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (searchLogs == null)
        {
          // searchLogs bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetSearchLogsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / searchLogs Bulunamadı",
              "Belirtilen ID'ye sahip searchLogs bulunamadı.",
              $"ID '{request.Id}' ile eşleşen searchLogs bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (searchLogs.AuthUserId.HasValue)
          allUserIds.Add(searchLogs.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (searchLogs.CreateUserId.HasValue)
          allUserIds.Add(searchLogs.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (searchLogs.UpdateUserId.HasValue)
          allUserIds.Add(searchLogs.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (searchLogs.AuthUserId.HasValue && allUserDetails.ContainsKey(searchLogs.AuthUserId.Value))
        {
          var userDetail = allUserDetails[searchLogs.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (searchLogs.CreateUserId.HasValue && allUserDetails.ContainsKey(searchLogs.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[searchLogs.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (searchLogs.UpdateUserId.HasValue && allUserDetails.ContainsKey(searchLogs.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[searchLogs.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // searchLogs detay DTO'su oluşturuluyor
        var searchLogsDetail = new SearchLogsDTO
        {
          Id = searchLogs.Id,
          AuthUserId = searchLogs.AuthUserId,
          AuthCustomerId = searchLogs.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = searchLogs.CreateUserId,
          UpdateUserId = searchLogs.UpdateUserId,
          RowCreatedDate = searchLogs.RowCreatedDate,
          RowUpdatedDate = searchLogs.RowUpdatedDate,
          RowIsActive = searchLogs.RowIsActive,
          RowIsDeleted = searchLogs.RowIsDeleted,
          // Search specific fields - Arama özel alanları
          SearchTerm = searchLogs.SearchTerm,
          SearchFilters = searchLogs.SearchFilters,
          ResultsCount = searchLogs.ResultsCount,
          SearchDate = searchLogs.SearchDate,
          IpAddress = searchLogs.IpAddress?.ToString(),
          UserAgent = searchLogs.UserAgent,
          Icon = searchLogs.Icon,
          // Additional computed fields - Ek hesaplanan alanlar
          SearchDateFormatted = searchLogs.SearchDate?.ToString("yyyy-MM-dd HH:mm:ss"),
          ResultsCountFormatted = searchLogs.ResultsCount?.ToString("N0")
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetSearchLogsByIdQueryResponse>(
            new GetSearchLogsByIdQueryResponse
            {
              searchLogs = searchLogsDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "searchLogs başarıyla getirildi.",
            $"searchLogs Id: {searchLogs.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetSearchLogsByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "searchLogs getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

