using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.SearchLogs;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.SearchLogs.GetAllSearchLogs
{
  public class GetAllSearchLogsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllSearchLogsQueryRequest, TransactionResultPack<GetAllSearchLogsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllSearchLogsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllSearchLogsQueryResponse>> Handle(GetAllSearchLogsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.searchLogs
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplySearchLogsFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var searchLogss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = searchLogss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = searchLogss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = searchLogss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var searchLogsDetails = new List<SearchLogsDTO>();  // 🎯 searchLogsDTO listesi oluştur

        foreach (var searchLogs in searchLogss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (searchLogs.AuthUserId.HasValue && allUserDetails.ContainsKey(searchLogs.AuthUserId.Value))
          {
            var userDetail = allUserDetails[searchLogs.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var searchLogsDetail = new SearchLogsDTO
          {
            Id = searchLogs.Id,
            AuthUserId = searchLogs.AuthUserId,
            AuthCustomerId = searchLogs.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = searchLogs.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = searchLogs.UpdateUserId,
            UpdateUserName = updateUserName,
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

          searchLogsDetails.Add(searchLogsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllSearchLogsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllSearchLogsQueryResponse
            {
              TotalCount = totalCount,
              searchLogs = searchLogsDetails  // 🎯 searchLogsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "searchLogs başarıyla getirildi.",
            $"searchLogss.Count searchLogs başarıyla getirildi."  // 🎯 searchLogs sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllSearchLogsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "searchLogs getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı SearchLogs filtrelerini uygular - Applies SearchLogs filters
    /// </summary>
    private IQueryable<SearchLogsViewModel> ApplySearchLogsFilters(
        IQueryable<SearchLogsViewModel> query,
        GetAllSearchLogsQueryRequest request)
    {
      // Search specific filters - Arama özel filtreleri
      if (!string.IsNullOrWhiteSpace(request.SearchTerm))
      {
        query = query.Where(x => x.SearchTerm.Contains(request.SearchTerm.Trim()));
      }

      if (!string.IsNullOrWhiteSpace(request.SearchFilters))
      {
        query = query.Where(x => x.SearchFilters != null && x.SearchFilters.Contains(request.SearchFilters.Trim()));
      }

      if (request.ResultsCountFrom.HasValue)
      {
        query = query.Where(x => x.ResultsCount >= request.ResultsCountFrom.Value);
      }

      if (request.ResultsCountTo.HasValue)
      {
        query = query.Where(x => x.ResultsCount <= request.ResultsCountTo.Value);
      }

      if (request.SearchDateFrom.HasValue)
      {
        query = query.Where(x => x.SearchDate >= request.SearchDateFrom.Value);
      }

      if (request.SearchDateTo.HasValue)
      {
        query = query.Where(x => x.SearchDate <= request.SearchDateTo.Value);
      }

      if (!string.IsNullOrWhiteSpace(request.IpAddress))
      {
        query = query.Where(x => x.IpAddress != null && x.IpAddress.ToString().Contains(request.IpAddress.Trim()));
      }

      if (!string.IsNullOrWhiteSpace(request.UserAgent))
      {
        query = query.Where(x => x.UserAgent != null && x.UserAgent.Contains(request.UserAgent.Trim()));
      }

      if (!string.IsNullOrWhiteSpace(request.Icon))
      {
        query = query.Where(x => x.Icon.Contains(request.Icon.Trim()));
      }

      // Date range filters - Tarih aralığı filtreleri
      if (request.CreatedDateFrom.HasValue)
      {
        query = query.Where(x => x.RowCreatedDate >= request.CreatedDateFrom.Value);
      }

      if (request.CreatedDateTo.HasValue)
      {
        query = query.Where(x => x.RowCreatedDate <= request.CreatedDateTo.Value);
      }

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
