using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.UserVisits;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.UserVisits.GetAllUserVisits
{
  public class GetAllUserVisitsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllUserVisitsQueryRequest, TransactionResultPack<GetAllUserVisitsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllUserVisitsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllUserVisitsQueryResponse>> Handle(GetAllUserVisitsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.userVisits
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyUserVisitsFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var userVisitss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = userVisitss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = userVisitss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = userVisitss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var userVisitsDetails = new List<UserVisitsDTO>();  // 🎯 userVisitsDTO listesi oluştur

        foreach (var userVisits in userVisitss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (userVisits.AuthUserId.HasValue && allUserDetails.ContainsKey(userVisits.AuthUserId.Value))
          {
            var userDetail = allUserDetails[userVisits.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
          }

          // 🎯 Create/Update kullanıcı bilgilerini al
          string? createUserName = null;
          string? updateUserName = null;

          if (userVisits.CreateUserId.HasValue && allUserDetails.ContainsKey(userVisits.CreateUserId.Value))
          {
            var createUserDetail = allUserDetails[userVisits.CreateUserId.Value];
            createUserName = createUserDetail.AuthUserName;
          }

          if (userVisits.UpdateUserId.HasValue && allUserDetails.ContainsKey(userVisits.UpdateUserId.Value))
          {
            var updateUserDetail = allUserDetails[userVisits.UpdateUserId.Value];
            updateUserName = updateUserDetail.AuthUserName;
          }

          var userVisitsDetail = new UserVisitsDTO
          {
            Id = userVisits.Id,
            BusinessId = userVisits.BusinessId,
            VisitDate = userVisits.VisitDate,
            VisitType = userVisits.VisitType,
            Icon = userVisits.Icon,
            AuthUserId = userVisits.AuthUserId,
            AuthCustomerId = userVisits.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = userVisits.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = userVisits.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = userVisits.RowCreatedDate,
            RowUpdatedDate = userVisits.RowUpdatedDate,
            RowIsActive = userVisits.RowIsActive,
            RowIsDeleted = userVisits.RowIsDeleted
          };

          userVisitsDetails.Add(userVisitsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllUserVisitsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllUserVisitsQueryResponse
            {
              TotalCount = totalCount,
              userVisits = userVisitsDetails  // 🎯 userVisitsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "userVisits başarıyla getirildi.",
            $"userVisitss.Count userVisits başarıyla getirildi."  // 🎯 userVisits sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllUserVisitsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "userVisits getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı ziyaret filtrelerini uygular - Applies user visits filters
    /// </summary>
    private IQueryable<UserVisitsViewModel> ApplyUserVisitsFilters(
        IQueryable<UserVisitsViewModel> query,
        GetAllUserVisitsQueryRequest request)
    {
      // 🏢 İşletme bilgileri filtreleri - Business information filters
      if (request.BusinessId.HasValue)
        query = query.Where(x => x.BusinessId == request.BusinessId.Value);

      if (request.VisitDate.HasValue)
        query = query.Where(x => x.VisitDate >= request.VisitDate.Value);

      if (!string.IsNullOrEmpty(request.VisitType))
        query = query.Where(x => x.VisitType != null && x.VisitType.Contains(request.VisitType));

      if (!string.IsNullOrEmpty(request.Icon))
        query = query.Where(x => x.Icon != null && x.Icon.Contains(request.Icon));

      return query;
    }
  }
}
