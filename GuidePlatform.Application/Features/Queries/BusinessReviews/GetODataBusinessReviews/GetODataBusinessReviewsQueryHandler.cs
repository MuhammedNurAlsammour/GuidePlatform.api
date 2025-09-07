using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.Enums;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Contexts;
using Karmed.External.Auth.Library.Services;
using Karmed.External.Auth.Library.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetODataBusinessReviews
{
  /// <summary>
  /// OData BusinessReviews için Handler
  /// </summary>
  public class GetODataBusinessReviewsQueryHandler : BaseQueryHandler, IRequestHandler<GetODataBusinessReviewsQueryRequest, TransactionResultPack<GetODataBusinessReviewsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly AuthDbContext _authContext;
    private readonly UserManager<AppUser> _userManager;

    public GetODataBusinessReviewsQueryHandler(
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

    public async Task<TransactionResultPack<GetODataBusinessReviewsQueryResponse>> Handle(GetODataBusinessReviewsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // Context kontrolleri
        if (_context == null)
          throw new InvalidOperationException("ApplicationDbContext null");

        if (_context.businessReviews == null)
          throw new InvalidOperationException("businessReviews DbSet null");

        if (_context.businesses == null)
          throw new InvalidOperationException("businesses DbSet null");


        IQueryable<BusinessReviewsDTO> query;

        // Check if we have entity-level filters - apply them at entity level for better performance
        if (request.HasFilter && (request.Filter.Contains("BusinessId eq") || request.Filter.Contains("Id eq") ||
                                  request.Filter.Contains("Rating eq") || request.Filter.Contains("IsVerified eq") ||
                                  request.Filter.Contains("IsApproved eq")))
        {
          query = GetBusinessReviewsODataQueryWithDirectFilter(request.Filter);

          // Apply other filters at DTO level
          var otherFilters = RemoveEntityLevelFilters(request.Filter);
          if (!string.IsNullOrEmpty(otherFilters))
          {
            var modifiedRequest = new GetODataBusinessReviewsQueryRequest
            {
              Filter = otherFilters,
              OrderBy = request.OrderBy,
              Top = request.Top,
              Skip = request.Skip,
              Count = request.Count,
              Select = request.Select
            };
            query = ApplyODataFilters(query, modifiedRequest);
          }
          else
          {
            // Apply non-filter OData operations
            query = ApplyODataFilters(query, new GetODataBusinessReviewsQueryRequest
            {
              OrderBy = request.OrderBy,
              Top = request.Top,
              Skip = request.Skip,
              Count = request.Count,
              Select = request.Select
            });
          }
        }
        else
        {
          // No BusinessId filter, use original approach
          query = GetBusinessReviewsODataQuery();
          query = ApplyODataFilters(query, request);
        }

        // Execute filtered query first to get the actual filtered results
        var filteredResults = await query.ToListAsync(cancellationToken);

        // Kullanıcı adlarını doldur (multiple context sorunu nedeniyle ayrı işlem)
        var enrichedResults = await EnrichWithUserNamesAsync(filteredResults, cancellationToken);

        // Apply $select if specified
        if (!string.IsNullOrEmpty(request.Select))
        {
          enrichedResults = ApplySelectToResults(enrichedResults, request.Select);
        }

        // Create response object
        var response = new GetODataBusinessReviewsQueryResponse
        {
          StatusCode = 200,
          Message = $"Success - Results: {enrichedResults.Count}",
          Timestamp = DateTime.UtcNow,
          TotalCount = enrichedResults.Count,
          BusinessReviews = enrichedResults
        };

        // TransactionResultPack ile sarmala
        var result = new TransactionResultPack<GetODataBusinessReviewsQueryResponse>
        {
          Result = response,
          OperationResult = new TransactionResult
          {
            Result = TransactionResultEnm.Success,
            MessageTitle = "İşlem Başarılı",
            MessageContent = "businessReviews başarıyla getirildi.",
            MessageDetail = $"{response.TotalCount} businessReviews başarıyla getirildi."
          }
        };

        return result;
      }
      catch (Exception ex)
      {
        var errorResponse = new GetODataBusinessReviewsQueryResponse
        {
          StatusCode = 400,
          Message = "Error",
          Timestamp = DateTime.UtcNow,
          TotalCount = 0,
          BusinessReviews = new List<BusinessReviewsDTO>()
        };

        var errorResult = new TransactionResultPack<GetODataBusinessReviewsQueryResponse>
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

        return errorResult;
      }
    }

    /// <summary>
    /// BusinessReviews OData query'sini oluşturur
    /// </summary>
    private IQueryable<BusinessReviewsDTO> GetBusinessReviewsODataQuery()
    {
      // BusinessReviews'ları Business bilgileri ile join et
      // Hem review hem de business için RowIsActive ve RowIsDeleted kontrolü
      // Kullanıcı adları ayrı olarak doldurulacak (multiple context sorunu nedeniyle)
      var query = from review in _context.businessReviews
                  join business in _context.businesses on review.BusinessId equals business.Id
                  where review.RowIsActive && !review.RowIsDeleted
                        && business.RowIsActive && !business.RowIsDeleted
                  select new BusinessReviewsDTO
                  {
                    Id = review.Id,
                    BusinessId = review.BusinessId,
                    BusinessName = business.Name,
                    ReviewerId = review.ReviewerId,
                    ReviewerName = null, // Ayrı olarak doldurulacak
                    Rating = review.Rating,
                    Comment = review.Comment,
                    IsVerified = review.IsVerified,
                    IsApproved = review.IsApproved,
                    Icon = review.Icon,
                    AuthUserId = review.AuthUserId,
                    AuthCustomerId = review.AuthCustomerId,
                    AuthUserName = null, // Ayrı olarak doldurulacak
                    AuthCustomerName = null, // Ayrı olarak doldurulacak
                    CreateUserId = review.CreateUserId,
                    CreateUserName = null, // Ayrı olarak doldurulacak
                    UpdateUserId = review.UpdateUserId,
                    UpdateUserName = null, // Ayrı olarak doldurulacak
                    RowCreatedDate = review.RowCreatedDate,
                    RowUpdatedDate = review.RowUpdatedDate,
                    RowIsActive = review.RowIsActive,
                    RowIsDeleted = review.RowIsDeleted
                  };

      return query;
    }

    /// <summary>
    /// ALTERNATIVE SOLUTION: Apply filters directly to the base query instead of DTO query
    /// </summary>
    private IQueryable<BusinessReviewsDTO> GetBusinessReviewsODataQueryWithDirectFilter(string filter)
    {
      // Start with the entity query and apply filters there
      var baseQuery = from review in _context.businessReviews
                      join business in _context.businesses on review.BusinessId equals business.Id
                      where review.RowIsActive && !review.RowIsDeleted
                            && business.RowIsActive && !business.RowIsDeleted
                      select new { review, business };

      // Apply entity-level filters directly to entity query if present
      if (!string.IsNullOrEmpty(filter))
      {
        // BusinessId filter
        if (filter.Contains("BusinessId eq"))
        {
          var parts = filter.Split(new string[] { " eq " }, 2, StringSplitOptions.None);
          if (parts.Length == 2 && parts[0].Trim().Equals("BusinessId", StringComparison.OrdinalIgnoreCase))
          {
            var businessIdValue = parts[1].Trim().Trim('\'');
            if (Guid.TryParse(businessIdValue, out Guid businessId))
            {
              baseQuery = baseQuery.Where(x => x.review.BusinessId == businessId);
            }
          }
        }

        // Id filter
        if (filter.Contains("Id eq"))
        {
          var parts = filter.Split(new string[] { " eq " }, 2, StringSplitOptions.None);
          if (parts.Length == 2 && parts[0].Trim().Equals("Id", StringComparison.OrdinalIgnoreCase))
          {
            var idValue = parts[1].Trim().Trim('\'');
            if (Guid.TryParse(idValue, out Guid id))
            {
              baseQuery = baseQuery.Where(x => x.review.Id == id);
            }
          }
        }

        // Rating filter
        if (filter.Contains("Rating eq"))
        {
          var parts = filter.Split(new string[] { " eq " }, 2, StringSplitOptions.None);
          if (parts.Length == 2 && parts[0].Trim().Equals("Rating", StringComparison.OrdinalIgnoreCase))
          {
            var ratingValue = parts[1].Trim().Trim('\'');
            if (int.TryParse(ratingValue, out int rating))
            {
              baseQuery = baseQuery.Where(x => x.review.Rating == rating);
            }
          }
        }

        // IsVerified filter
        if (filter.Contains("IsVerified eq"))
        {
          var parts = filter.Split(new string[] { " eq " }, 2, StringSplitOptions.None);
          if (parts.Length == 2 && parts[0].Trim().Equals("IsVerified", StringComparison.OrdinalIgnoreCase))
          {
            var isVerifiedValue = parts[1].Trim().Trim('\'');
            if (bool.TryParse(isVerifiedValue, out bool isVerified))
            {
              baseQuery = baseQuery.Where(x => x.review.IsVerified == isVerified);
            }
          }
        }

        // IsApproved filter
        if (filter.Contains("IsApproved eq"))
        {
          var parts = filter.Split(new string[] { " eq " }, 2, StringSplitOptions.None);
          if (parts.Length == 2 && parts[0].Trim().Equals("IsApproved", StringComparison.OrdinalIgnoreCase))
          {
            var isApprovedValue = parts[1].Trim().Trim('\'');
            if (bool.TryParse(isApprovedValue, out bool isApproved))
            {
              baseQuery = baseQuery.Where(x => x.review.IsApproved == isApproved);
            }
          }
        }
      }

      // Now project to DTO
      var dtoQuery = baseQuery.Select(x => new BusinessReviewsDTO
      {
        Id = x.review.Id,
        BusinessId = x.review.BusinessId,
        BusinessName = x.business.Name,
        ReviewerId = x.review.ReviewerId,
        ReviewerName = null, // Will be filled later
        Rating = x.review.Rating,
        Comment = x.review.Comment,
        IsVerified = x.review.IsVerified,
        IsApproved = x.review.IsApproved,
        Icon = x.review.Icon,
        AuthUserId = x.review.AuthUserId,
        AuthCustomerId = x.review.AuthCustomerId,
        AuthUserName = null, // Will be filled later
        AuthCustomerName = null, // Will be filled later
        CreateUserId = x.review.CreateUserId,
        CreateUserName = null, // Will be filled later
        UpdateUserId = x.review.UpdateUserId,
        UpdateUserName = null, // Will be filled later
        RowCreatedDate = x.review.RowCreatedDate,
        RowUpdatedDate = x.review.RowUpdatedDate,
        RowIsActive = x.review.RowIsActive,
        RowIsDeleted = x.review.RowIsDeleted
      });

      return dtoQuery;
    }

    /// <summary>
    /// Helper method to remove entity-level filters from filter string
    /// </summary>
    private string RemoveEntityLevelFilters(string filter)
    {
      if (string.IsNullOrEmpty(filter)) return filter;

      var result = filter;

      // Remove entity-level filters that are handled at entity level
      var patterns = new[]
      {
        @"BusinessId\s+eq\s+'[^']+'\s*",
        @"Id\s+eq\s+'[^']+'\s*",
        @"Rating\s+eq\s+\d+\s*",
        @"IsVerified\s+eq\s+(true|false)\s*",
        @"IsApproved\s+eq\s+(true|false)\s*"
      };

      foreach (var pattern in patterns)
      {
        result = System.Text.RegularExpressions.Regex.Replace(result, pattern, "",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
      }

      // Clean up any leftover 'and' or 'or' at the beginning
      result = System.Text.RegularExpressions.Regex.Replace(result, @"^(and|or)\s+", "",
          System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();

      return result;
    }

    /// <summary>
    /// Kullanıcı adlarını ayrı context ile doldurur (multiple context sorunu nedeniyle)
    /// </summary>
    private async Task<List<BusinessReviewsDTO>> EnrichWithUserNamesAsync(List<BusinessReviewsDTO> results, CancellationToken cancellationToken)
    {
      try
      {
        // Tüm user ID'leri topla
        var allUserIds = new HashSet<Guid>();

        foreach (var item in results)
        {
          if (item.AuthUserId.HasValue) allUserIds.Add(item.AuthUserId.Value);
          if (item.CreateUserId.HasValue) allUserIds.Add(item.CreateUserId.Value);
          if (item.UpdateUserId.HasValue) allUserIds.Add(item.UpdateUserId.Value);
          if (item.ReviewerId != Guid.Empty) allUserIds.Add(item.ReviewerId);
        }

        // UserManager ile kullanıcı bilgilerini al
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

        // AuthUserDetailDto'dan customer bilgilerini al (AuthCustomerName için)
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

        // Kullanıcı adlarını doldur
        foreach (var item in results)
        {
          item.ReviewerName = item.ReviewerId != Guid.Empty && userDetails.ContainsKey(item.ReviewerId) ? userDetails[item.ReviewerId] : null;
          item.AuthUserName = item.AuthUserId.HasValue && userDetails.ContainsKey(item.AuthUserId.Value) ? userDetails[item.AuthUserId.Value] : null;
          item.AuthCustomerName = item.AuthUserId.HasValue && authCustomerDetails.ContainsKey(item.AuthUserId.Value) ? authCustomerDetails[item.AuthUserId.Value] : null;
          item.CreateUserName = item.CreateUserId.HasValue && userDetails.ContainsKey(item.CreateUserId.Value) ? userDetails[item.CreateUserId.Value] : null;
          item.UpdateUserName = item.UpdateUserId.HasValue && userDetails.ContainsKey(item.UpdateUserId.Value) ? userDetails[item.UpdateUserId.Value] : null;
        }

        return results;
      }
      catch (Exception)
      {
        // Hata durumunda orijinal results'u döndür
        return results;
      }
    }

    /// <summary>
    /// OData filters'ı uygular - Comprehensive OData support
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplyODataFilters(IQueryable<BusinessReviewsDTO> query, GetODataBusinessReviewsQueryRequest request)
    {
      // Apply filters with comprehensive OData support
      if (!string.IsNullOrEmpty(request.Filter))
      {
        query = ApplyFilterExpression(query, request.Filter);
      }

      // Apply ordering
      if (!string.IsNullOrEmpty(request.OrderBy))
      {
        query = ApplyOrderBy(query, request.OrderBy);
      }

      // Apply paging
      if (request.Skip.HasValue && request.Skip > 0)
      {
        query = query.Skip(request.Skip.Value);
      }

      if (request.Top.HasValue && request.Top > 0)
      {
        query = query.Take(request.Top.Value);
      }

      // Apply $select if specified
      if (!string.IsNullOrEmpty(request.Select))
      {
        query = ApplySelect(query, request.Select);
      }

      return query;
    }

    /// <summary>
    /// $select uygular - BusinessReviews için özel implementasyon
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplySelect(IQueryable<BusinessReviewsDTO> query, string select)
    {
      if (string.IsNullOrEmpty(select))
        return query;

      // Field selection will be handled at the serialization level
      return query;
    }

    /// <summary>
    /// $select uygular - Results üzerinde field selection
    /// </summary>
    private List<BusinessReviewsDTO> ApplySelectToResults(List<BusinessReviewsDTO> results, string select)
    {
      if (string.IsNullOrEmpty(select))
        return results;

      // Parse the select fields
      var fields = select.Split(',').Select(f => f.Trim()).ToList();

      // Create new DTOs with only the selected fields
      var selectedResults = new List<BusinessReviewsDTO>();

      foreach (var result in results)
      {
        var selectedResult = new BusinessReviewsDTO();

        // Only set the selected fields
        foreach (var field in fields)
        {
          switch (field.ToLower())
          {
            case "id":
              selectedResult.Id = result.Id;
              break;
            case "businessid":
              selectedResult.BusinessId = result.BusinessId;
              break;
            case "businessname":
              selectedResult.BusinessName = result.BusinessName;
              break;
            case "reviewerid":
              selectedResult.ReviewerId = result.ReviewerId;
              break;
            case "reviewername":
              selectedResult.ReviewerName = result.ReviewerName;
              break;
            case "rating":
              selectedResult.Rating = result.Rating;
              break;
            case "comment":
              selectedResult.Comment = result.Comment;
              break;
            case "isverified":
              selectedResult.IsVerified = result.IsVerified;
              break;
            case "isapproved":
              selectedResult.IsApproved = result.IsApproved;
              break;
            case "icon":
              selectedResult.Icon = result.Icon;
              break;
            case "authuserid":
              selectedResult.AuthUserId = result.AuthUserId;
              break;
            case "authusername":
              selectedResult.AuthUserName = result.AuthUserName;
              break;
            case "authcustomerid":
              selectedResult.AuthCustomerId = result.AuthCustomerId;
              break;
            case "authcustomername":
              selectedResult.AuthCustomerName = result.AuthCustomerName;
              break;
            case "createuserid":
              selectedResult.CreateUserId = result.CreateUserId;
              break;
            case "createusername":
              selectedResult.CreateUserName = result.CreateUserName;
              break;
            case "updateuserid":
              selectedResult.UpdateUserId = result.UpdateUserId;
              break;
            case "updateusername":
              selectedResult.UpdateUserName = result.UpdateUserName;
              break;
            case "rowcreateddate":
              selectedResult.RowCreatedDate = result.RowCreatedDate;
              break;
            case "rowupdateddate":
              selectedResult.RowUpdatedDate = result.RowUpdatedDate;
              break;
            case "rowisactive":
              selectedResult.RowIsActive = result.RowIsActive;
              break;
            case "rowisdeleted":
              selectedResult.RowIsDeleted = result.RowIsDeleted;
              break;
          }
        }

        selectedResults.Add(selectedResult);
      }

      return selectedResults;
    }

    /// <summary>
    /// OrderBy uygular - BusinessReviews için özel implementasyon
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplyOrderBy(IQueryable<BusinessReviewsDTO> query, string orderBy)
    {
      if (orderBy.Contains("Rating desc"))
      {
        query = query.OrderByDescending(x => x.Rating);
      }
      else if (orderBy.Contains("Rating asc"))
      {
        query = query.OrderBy(x => x.Rating);
      }
      else if (orderBy.Contains("RowCreatedDate desc"))
      {
        query = query.OrderByDescending(x => x.RowCreatedDate);
      }
      else if (orderBy.Contains("RowCreatedDate asc"))
      {
        query = query.OrderBy(x => x.RowCreatedDate);
      }
      else if (orderBy.Contains("BusinessName desc"))
      {
        query = query.OrderByDescending(x => x.BusinessName);
      }
      else if (orderBy.Contains("BusinessName asc"))
      {
        query = query.OrderBy(x => x.BusinessName);
      }
      else
      {
        // Default ordering
        query = query.OrderByDescending(x => x.RowCreatedDate);
      }

      return query;
    }

    /// <summary>
    /// Filter expression parser
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplyFilterExpression(IQueryable<BusinessReviewsDTO> query, string filter)
    {
      try
      {
        // Handle multiple conditions with 'and' and 'or'
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
            // Note: OR operations are complex with IQueryable, this is a simplified approach
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
        // If parsing fails, return original query
        return query;
      }
    }

    /// <summary>
    /// Apply single filter condition
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplySingleFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {

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
        return query;
      }
    }

    /// <summary>
    /// Contains filter uygular - BusinessReviews için özel implementasyon
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplyContainsFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      var match = System.Text.RegularExpressions.Regex.Match(condition, @"contains\((\w+),\s*'([^']+)'\)");
      if (match.Success)
      {
        var property = match.Groups[1].Value;
        var value = match.Groups[2].Value;

        return property.ToLower() switch
        {
          "businessname" => query.Where(x => x.BusinessName.Contains(value)),
          "comment" => query.Where(x => x.Comment.Contains(value)),
          "reviewername" => query.Where(x => x.ReviewerName != null && x.ReviewerName.Contains(value)),
          "authusername" => query.Where(x => x.AuthUserName != null && x.AuthUserName.Contains(value)),
          "authcustomername" => query.Where(x => x.AuthCustomerName != null && x.AuthCustomerName.Contains(value)),
          "createusername" => query.Where(x => x.CreateUserName != null && x.CreateUserName.Contains(value)),
          "updateusername" => query.Where(x => x.UpdateUserName != null && x.UpdateUserName.Contains(value)),
          "businessid" => query.Where(x => x.BusinessId.ToString().Contains(value)),
          "reviewerid" => query.Where(x => x.ReviewerId.ToString().Contains(value)),
          "id" => query.Where(x => x.Id.ToString().Contains(value)),
          _ => query
        };
      }
      return query;
    }

    /// <summary>
    /// Equals filter uygular - BusinessReviews için özel implementasyon
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplyEqualsFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      var parts = condition.Split(new string[] { " eq " }, 2, StringSplitOptions.None);
      if (parts.Length == 2)
      {
        var property = parts[0].Trim();
        var value = parts[1].Trim().Trim('\'');

        var result = property.ToLower() switch
        {
          "businessid" => query.Where(x => x.BusinessId == Guid.Parse(value)),
          "businessname" => query.Where(x => x.BusinessName == value),
          "rating" => query.Where(x => x.Rating == int.Parse(value)),
          "isverified" => query.Where(x => x.IsVerified == bool.Parse(value)),
          "isapproved" => query.Where(x => x.IsApproved == bool.Parse(value)),
          "reviewerid" => query.Where(x => x.ReviewerId == Guid.Parse(value)),
          "reviewername" => query.Where(x => x.ReviewerName == value),
          "authuserid" => query.Where(x => x.AuthUserId == Guid.Parse(value)),
          "authusername" => query.Where(x => x.AuthUserName == value),
          "authcustomerid" => query.Where(x => x.AuthCustomerId == Guid.Parse(value)),
          "authcustomername" => query.Where(x => x.AuthCustomerName == value),
          "createuserid" => query.Where(x => x.CreateUserId == Guid.Parse(value)),
          "createusername" => query.Where(x => x.CreateUserName == value),
          "updateuserid" => query.Where(x => x.UpdateUserId == Guid.Parse(value)),
          "updateusername" => query.Where(x => x.UpdateUserName == value),
          "rowisactive" => query.Where(x => x.RowIsActive == bool.Parse(value)),
          "rowisdeleted" => query.Where(x => x.RowIsDeleted == bool.Parse(value)),
          _ => query
        };

        return result;
      }
      return query;
    }

    /// <summary>
    /// Greater than filter uygular - BusinessReviews için özel implementasyon
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplyGreaterThanFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      var parts = condition.Split(new string[] { " gt " }, 2, StringSplitOptions.None);
      if (parts.Length == 2)
      {
        var property = parts[0].Trim();
        var value = parts[1].Trim();

        return property.ToLower() switch
        {
          "rating" => query.Where(x => x.Rating > int.Parse(value)),
          "rowcreateddate" => query.Where(x => x.RowCreatedDate > DateTime.Parse(value.Trim('\''))),
          "rowupdateddate" => query.Where(x => x.RowUpdatedDate > DateTime.Parse(value.Trim('\''))),
          _ => query
        };
      }
      return query;
    }

    /// <summary>
    /// Greater than or equal filter uygular - BusinessReviews için özel implementasyon
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplyGreaterThanOrEqualFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      var parts = condition.Split(new string[] { " ge " }, 2, StringSplitOptions.None);
      if (parts.Length == 2)
      {
        var property = parts[0].Trim();
        var value = parts[1].Trim();

        return property.ToLower() switch
        {
          "rating" => query.Where(x => x.Rating >= int.Parse(value)),
          "rowcreateddate" => query.Where(x => x.RowCreatedDate >= DateTime.Parse(value.Trim('\''))),
          "rowupdateddate" => query.Where(x => x.RowUpdatedDate >= DateTime.Parse(value.Trim('\''))),
          _ => query
        };
      }
      return query;
    }

    /// <summary>
    /// Less than filter uygular - BusinessReviews için özel implementasyon
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplyLessThanFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      var parts = condition.Split(new string[] { " lt " }, 2, StringSplitOptions.None);
      if (parts.Length == 2)
      {
        var property = parts[0].Trim();
        var value = parts[1].Trim();

        return property.ToLower() switch
        {
          "rating" => query.Where(x => x.Rating < int.Parse(value)),
          "rowcreateddate" => query.Where(x => x.RowCreatedDate < DateTime.Parse(value.Trim('\''))),
          "rowupdateddate" => query.Where(x => x.RowUpdatedDate < DateTime.Parse(value.Trim('\''))),
          _ => query
        };
      }
      return query;
    }

    /// <summary>
    /// Less than or equal filter uygular - BusinessReviews için özel implementasyon
    /// </summary>
    private IQueryable<BusinessReviewsDTO> ApplyLessThanOrEqualFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      var parts = condition.Split(new string[] { " le " }, 2, StringSplitOptions.None);
      if (parts.Length == 2)
      {
        var property = parts[0].Trim();
        var value = parts[1].Trim();

        return property.ToLower() switch
        {
          "rating" => query.Where(x => x.Rating <= int.Parse(value)),
          "rowcreateddate" => query.Where(x => x.RowCreatedDate <= DateTime.Parse(value.Trim('\''))),
          "rowupdateddate" => query.Where(x => x.RowUpdatedDate <= DateTime.Parse(value.Trim('\''))),
          _ => query
        };
      }
      return query;
    }

    // Additional filter methods for other operations
    private IQueryable<BusinessReviewsDTO> ApplyStartsWithFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      var match = System.Text.RegularExpressions.Regex.Match(condition, @"startswith\((\w+),\s*'([^']+)'\)");
      if (match.Success)
      {
        var property = match.Groups[1].Value;
        var value = match.Groups[2].Value;

        return property.ToLower() switch
        {
          "businessname" => query.Where(x => x.BusinessName.StartsWith(value)),
          "comment" => query.Where(x => x.Comment.StartsWith(value)),
          "reviewername" => query.Where(x => x.ReviewerName != null && x.ReviewerName.StartsWith(value)),
          _ => query
        };
      }
      return query;
    }

    private IQueryable<BusinessReviewsDTO> ApplyEndsWithFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      var match = System.Text.RegularExpressions.Regex.Match(condition, @"endswith\((\w+),\s*'([^']+)'\)");
      if (match.Success)
      {
        var property = match.Groups[1].Value;
        var value = match.Groups[2].Value;

        return property.ToLower() switch
        {
          "businessname" => query.Where(x => x.BusinessName.EndsWith(value)),
          "comment" => query.Where(x => x.Comment.EndsWith(value)),
          "reviewername" => query.Where(x => x.ReviewerName != null && x.ReviewerName.EndsWith(value)),
          _ => query
        };
      }
      return query;
    }

    private IQueryable<BusinessReviewsDTO> ApplyNotEqualsFilter(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      var parts = condition.Split(new string[] { " ne " }, 2, StringSplitOptions.None);
      if (parts.Length == 2)
      {
        var property = parts[0].Trim();
        var value = parts[1].Trim().Trim('\'');

        return property.ToLower() switch
        {
          "businessid" => query.Where(x => x.BusinessId != Guid.Parse(value)),
          "businessname" => query.Where(x => x.BusinessName != value),
          "rating" => query.Where(x => x.Rating != int.Parse(value)),
          "isverified" => query.Where(x => x.IsVerified != bool.Parse(value)),
          "isapproved" => query.Where(x => x.IsApproved != bool.Parse(value)),
          _ => query
        };
      }
      return query;
    }

    private IQueryable<BusinessReviewsDTO> ApplySimpleCondition(IQueryable<BusinessReviewsDTO> query, string condition)
    {
      // Default implementation - her entity için override edilmeli
      return query;
    }
  }
}