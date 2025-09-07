using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.Enums;
using GuidePlatform.Application.Dtos.ResponseDtos;
using Karmed.External.Auth.Library.Contexts;
using Karmed.External.Auth.Library.Services;
using Karmed.External.Auth.Library.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace GuidePlatform.Application.Features.Queries.Base
{
  /// <summary>
  /// OData query'leri için base handler class
  /// Tüm OData işlemlerini yönetir
  /// </summary>
  public abstract class BaseODataQueryHandler<TRequest, TResponse, TDto> : BaseQueryHandler
    where TRequest : BaseODataQueryRequest<TResponse>
    where TResponse : BaseODataQueryResponse<TDto>, new()
    where TDto : BaseResponseDTO
  {
    protected readonly IApplicationDbContext _context;
    protected readonly AuthDbContext _authContext;
    protected readonly UserManager<AppUser> _userManager;

    protected BaseODataQueryHandler(
        IApplicationDbContext context,
        AuthDbContext authContext,
        UserManager<AppUser> userManager,
        ICurrentUserService currentUserService,
        IAuthUserDetailService authUserService) : base(currentUserService, authUserService)
    {
      _context = context;
      _authContext = authContext;
      _userManager = userManager;
    }

    /// <summary>
    /// Ana handle metodu
    /// </summary>
    public async Task<TransactionResultPack<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // Context kontrolleri
        ValidateContexts();

        // Base OData query'sini oluştur
        var query = GetBaseODataQuery();

        // Debug logging
        System.Diagnostics.Debug.WriteLine($"Handler received filter: '{request.Filter}'");

        // OData filtrelerini uygula
        var filteredQuery = ApplyODataFilters(query, request);
        System.Diagnostics.Debug.WriteLine($"Filtered query type: {filteredQuery.GetType().Name}");

        // Filtrelenmiş query'yi çalıştır
        var filteredResults = await filteredQuery.ToListAsync(cancellationToken);
        System.Diagnostics.Debug.WriteLine($"Filtered results count: {filteredResults.Count}");

        // Kullanıcı adlarını doldur
        var enrichedResults = await EnrichWithUserNamesAsync(filteredResults, cancellationToken);

        // Response oluştur
        var response = CreateSuccessResponse(enrichedResults, enrichedResults.Count);

        // TransactionResultPack ile sarmala
        return CreateSuccessResult(response, enrichedResults.Count);
      }
      catch (Exception ex)
      {
        return CreateErrorResult(ex);
      }
    }

    /// <summary>
    /// Base OData query'sini oluşturur - Her entity için override edilmeli
    /// </summary>
    protected abstract IQueryable<TDto> GetBaseODataQuery();

    /// <summary>
    /// Context'leri doğrular
    /// </summary>
    protected virtual void ValidateContexts()
    {
      if (_context == null)
        throw new InvalidOperationException("ApplicationDbContext null");

      if (_authContext == null)
        throw new InvalidOperationException("AuthDbContext null");

      if (_userManager == null)
        throw new InvalidOperationException("UserManager null");
    }

    /// <summary>
    /// Kullanıcı adlarını doldurur - Her entity için override edilebilir
    /// </summary>
    protected virtual async Task<List<TDto>> EnrichWithUserNamesAsync(List<TDto> results, CancellationToken cancellationToken)
    {
      try
      {
        // Tüm user ID'leri topla
        var allUserIds = ExtractAllUserIds(results);

        // UserManager ile kullanıcı bilgilerini al
        var userDetails = await GetUserDetailsAsync(allUserIds, cancellationToken);

        // AuthUserDetailDto'dan customer bilgilerini al
        var authCustomerDetails = await GetAuthCustomerDetailsAsync(allUserIds, cancellationToken);

        // Kullanıcı adlarını doldur
        EnrichUserNames(results, userDetails, authCustomerDetails);

        return results;
      }
      catch (Exception)
      {
        // Hata durumunda orijinal results'u döndür
        return results;
      }
    }

    /// <summary>
    /// Tüm user ID'leri çıkarır - Her entity için override edilebilir
    /// </summary>
    protected virtual HashSet<Guid> ExtractAllUserIds(List<TDto> results)
    {
      var allUserIds = new HashSet<Guid>();

      foreach (var item in results)
      {
        if (item.AuthUserId.HasValue) allUserIds.Add(item.AuthUserId.Value);
        if (item.CreateUserId.HasValue) allUserIds.Add(item.CreateUserId.Value);
        if (item.UpdateUserId.HasValue) allUserIds.Add(item.UpdateUserId.Value);
      }

      return allUserIds;
    }

    /// <summary>
    /// User details'leri alır
    /// </summary>
    protected async Task<Dictionary<Guid, string>> GetUserDetailsAsync(HashSet<Guid> allUserIds, CancellationToken cancellationToken)
    {
      var userDetails = new Dictionary<Guid, string>();
      if (allUserIds.Any())
      {
        var users = await _userManager.Users
          .Where(u => allUserIds.Contains(u.Id))
          .Select(u => new { u.Id, u.UserName })
          .ToListAsync(cancellationToken);

        foreach (var user in users)
        {
          userDetails[user.Id] = user.UserName;
        }
      }

      return userDetails;
    }

    /// <summary>
    /// Auth customer details'leri alır
    /// </summary>
    protected async Task<Dictionary<Guid, string>> GetAuthCustomerDetailsAsync(HashSet<Guid> allUserIds, CancellationToken cancellationToken)
    {
      var authCustomerDetails = new Dictionary<Guid, string>();

      if (allUserIds.Any())
      {
        var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.ToList(), cancellationToken);
        foreach (var userDetail in authUserDetails.Values)
        {
          if (!string.IsNullOrEmpty(userDetail.AuthCustomerName))
          {
            authCustomerDetails[userDetail.AuthUserId] = userDetail.AuthCustomerName;
          }
        }
      }

      return authCustomerDetails;
    }

    /// <summary>
    /// Kullanıcı adlarını doldurur - Her entity için override edilebilir
    /// </summary>
    protected virtual void EnrichUserNames(List<TDto> results, Dictionary<Guid, string> userDetails, Dictionary<Guid, string> authCustomerDetails)
    {
      foreach (var item in results)
      {
        item.AuthUserName = item.AuthUserId.HasValue && userDetails.ContainsKey(item.AuthUserId.Value) ? userDetails[item.AuthUserId.Value] : null;
        item.AuthCustomerName = item.AuthUserId.HasValue && authCustomerDetails.ContainsKey(item.AuthUserId.Value) ? authCustomerDetails[item.AuthUserId.Value] : null;
        item.CreateUserName = item.CreateUserId.HasValue && userDetails.ContainsKey(item.CreateUserId.Value) ? userDetails[item.CreateUserId.Value] : null;
        item.UpdateUserName = item.UpdateUserId.HasValue && userDetails.ContainsKey(item.UpdateUserId.Value) ? userDetails[item.UpdateUserId.Value] : null;
      }
    }

    /// <summary>
    /// OData filtrelerini uygular
    /// </summary>
    protected virtual IQueryable<TDto> ApplyODataFilters(IQueryable<TDto> query, TRequest request)
    {
      // Filter uygula
      if (request.HasFilter)
      {
        System.Diagnostics.Debug.WriteLine($"Applying filter: {request.Filter}");
        query = ApplyFilterExpression(query, request.Filter);
        System.Diagnostics.Debug.WriteLine($"Filter applied. Query type: {query.GetType().Name}");
      }

      // OrderBy uygula
      if (request.HasOrderBy)
      {
        query = ApplyOrderBy(query, request.OrderBy);
      }

      // Paging uygula
      if (request.Skip.HasValue && request.Skip > 0)
      {
        query = query.Skip(request.Skip.Value);
      }

      if (request.Top.HasValue && request.Top > 0)
      {
        query = query.Take(request.Top.Value);
      }

      return query;
    }

    /// <summary>
    /// OrderBy uygular - Her entity için override edilebilir
    /// </summary>
    protected virtual IQueryable<TDto> ApplyOrderBy(IQueryable<TDto> query, string orderBy)
    {
      // Default implementation - her entity için override edilmeli
      return query.OrderByDescending(x => x.RowCreatedDate);
    }

    /// <summary>
    /// Filter expression parser
    /// </summary>
    protected virtual IQueryable<TDto> ApplyFilterExpression(IQueryable<TDto> query, string filter)
    {
      try
      {
        System.Diagnostics.Debug.WriteLine($"ApplyFilterExpression: filter='{filter}'");

        // Multiple conditions with 'and' and 'or'
        if (filter.Contains(" and "))
        {
          var conditions = filter.Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries);
          foreach (var condition in conditions)
          {
            query = ApplySingleFilter(query, condition.Trim());
          }
        }
        else if (filter.Contains(" or "))
        {
          var conditions = filter.Split(new[] { " or " }, StringSplitOptions.RemoveEmptyEntries);
          var firstQuery = ApplySingleFilter(query, conditions[0].Trim());
          for (int i = 1; i < conditions.Length; i++)
          {
            var orQuery = ApplySingleFilter(query, conditions[i].Trim());
            query = firstQuery.Union(orQuery);
          }
        }
        else
        {
          query = ApplySingleFilter(query, filter);
        }

        return query;
      }
      catch (Exception)
      {
        return query;
      }
    }

    /// <summary>
    /// Single filter condition uygular - Her entity için override edilebilir
    /// </summary>
    protected virtual IQueryable<TDto> ApplySingleFilter(IQueryable<TDto> query, string condition)
    {
      System.Diagnostics.Debug.WriteLine($"ApplySingleFilter: condition='{condition}'");

      // String operations
      if (condition.Contains("contains("))
      {
        return ApplyContainsFilter(query, condition);
      }
      else if (condition.Contains("startswith("))
      {
        return ApplyStartsWithFilter(query, condition);
      }
      else if (condition.Contains("endswith("))
      {
        return ApplyEndsWithFilter(query, condition);
      }
      // Comparison operations
      else if (condition.Contains(" eq "))
      {
        return ApplyEqualsFilter(query, condition);
      }
      else if (condition.Contains(" ne "))
      {
        return ApplyNotEqualsFilter(query, condition);
      }
      else if (condition.Contains(" gt "))
      {
        return ApplyGreaterThanFilter(query, condition);
      }
      else if (condition.Contains(" ge "))
      {
        return ApplyGreaterThanOrEqualFilter(query, condition);
      }
      else if (condition.Contains(" lt "))
      {
        return ApplyLessThanFilter(query, condition);
      }
      else if (condition.Contains(" le "))
      {
        return ApplyLessThanOrEqualFilter(query, condition);
      }
      // Default: try to parse as simple condition
      else
      {
        return ApplySimpleCondition(query, condition);
      }
    }

    #region Filter Implementation Methods - Her entity için override edilebilir

    protected virtual IQueryable<TDto> ApplyContainsFilter(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    protected virtual IQueryable<TDto> ApplyStartsWithFilter(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    protected virtual IQueryable<TDto> ApplyEndsWithFilter(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    protected virtual IQueryable<TDto> ApplyEqualsFilter(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    protected virtual IQueryable<TDto> ApplyNotEqualsFilter(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    protected virtual IQueryable<TDto> ApplyGreaterThanFilter(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    protected virtual IQueryable<TDto> ApplyGreaterThanOrEqualFilter(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    protected virtual IQueryable<TDto> ApplyLessThanFilter(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    protected virtual IQueryable<TDto> ApplyLessThanOrEqualFilter(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    protected virtual IQueryable<TDto> ApplySimpleCondition(IQueryable<TDto> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }

    #endregion

    /// <summary>
    /// Başarılı result oluşturur
    /// </summary>
    protected virtual TransactionResultPack<TResponse> CreateSuccessResult(TResponse response, int count)
    {
      return new TransactionResultPack<TResponse>
      {
        Result = response,
        OperationResult = new TransactionResult
        {
          Result = TransactionResultEnm.Success,
          MessageTitle = "İşlem Başarılı",
          MessageContent = "Veriler başarıyla getirildi.",
          MessageDetail = $"{count} kayıt başarıyla getirildi."
        }
      };
    }

    /// <summary>
    /// Hata result'u oluşturur
    /// </summary>
    protected virtual TransactionResultPack<TResponse> CreateErrorResult(Exception ex)
    {
      var errorResponse = CreateErrorResponse("Error", 400);

      return new TransactionResultPack<TResponse>
      {
        Result = errorResponse,
        OperationResult = new TransactionResult
        {
          Result = TransactionResultEnm.Error,
          MessageTitle = "Hata",
          MessageContent = "OData query oluşturulurken hata oluştu",
          MessageDetail = ex.Message + " | InnerException: " + (ex.InnerException?.Message ?? "None")
        }
      };
    }

    /// <summary>
    /// Başarılı response oluşturur
    /// </summary>
    protected virtual TResponse CreateSuccessResponse(List<TDto> data, int totalCount = 0, int? count = null)
    {
      var response = new TResponse
      {
        StatusCode = 200,
        Message = "Success",
        Timestamp = DateTime.UtcNow,
        TotalCount = totalCount > 0 ? totalCount : data.Count,
        Data = data,
        Count = count
      };
      return response;
    }

    /// <summary>
    /// Hata response'u oluşturur
    /// </summary>
    protected virtual TResponse CreateErrorResponse(string message, int statusCode = 400)
    {
      var response = new TResponse
      {
        StatusCode = statusCode,
        Message = message,
        Timestamp = DateTime.UtcNow,
        TotalCount = 0,
        Data = new List<TDto>()
      };
      return response;
    }
  }
}
