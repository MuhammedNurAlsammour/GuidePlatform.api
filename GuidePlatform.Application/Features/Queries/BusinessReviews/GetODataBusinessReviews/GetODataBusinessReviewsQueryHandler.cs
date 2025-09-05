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
  public class GetODataBusinessReviewsQueryHandler : BaseQueryHandler, IRequestHandler<GetODataBusinessReviewsQueryRequest, TransactionResultPack<IQueryable<BusinessReviewsDTO>>>
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

    public async Task<TransactionResultPack<IQueryable<BusinessReviewsDTO>>> Handle(GetODataBusinessReviewsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // Debug: Context kontrolü
        if (_context == null)
        {
          throw new InvalidOperationException("ApplicationDbContext null");
        }

        if (_context.businessReviews == null)
        {
          throw new InvalidOperationException("businessReviews DbSet null");
        }

        if (_context.businesses == null)
        {
          throw new InvalidOperationException("businesses DbSet null");
        }

        // BusinessReviews OData query'sini oluştur
        var query = GetBusinessReviewsODataQuery();

        // Kullanıcı adlarını doldur (multiple context sorunu nedeniyle ayrı işlem)
        var enrichedQuery = await EnrichWithUserNamesAsync(query, cancellationToken);

        // TransactionResultPack ile sarmala
        var result = new TransactionResultPack<IQueryable<BusinessReviewsDTO>>
        {
          Result = enrichedQuery,
          OperationResult = new TransactionResult
          {
            Result = TransactionResultEnm.Success,
            MessageTitle = "Başarılı",
            MessageContent = "OData query başarıyla oluşturuldu",
            MessageDetail = "BusinessReviews OData sorgusu hazır"
          }
        };

        return result;
      }
      catch (Exception ex)
      {
        var errorResult = new TransactionResultPack<IQueryable<BusinessReviewsDTO>>
        {
          Result = null,
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
    /// Kullanıcı adlarını ayrı context ile doldurur (multiple context sorunu nedeniyle)
    /// OData filter desteği için IQueryable döndürür
    /// </summary>
    private async Task<IQueryable<BusinessReviewsDTO>> EnrichWithUserNamesAsync(IQueryable<BusinessReviewsDTO> query, CancellationToken cancellationToken)
    {
      try
      {
        // Tüm user ID'leri topla (query execute etmeden)
        var allUserIds = new HashSet<Guid>();
        
        // Sadece ID'leri al, tüm veriyi değil
        var userIdsQuery = query.Select(x => new { 
          AuthUserId = x.AuthUserId, 
          CreateUserId = x.CreateUserId, 
          UpdateUserId = x.UpdateUserId, 
          ReviewerId = x.ReviewerId 
        });
        
        var userIds = await userIdsQuery.Take(1000).ToListAsync(cancellationToken);
        
        foreach (var item in userIds)
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

        // OData filter desteği için IQueryable döndür - kullanıcı adlarını projection ile ekle
        var enrichedQuery = query.Select(x => new BusinessReviewsDTO
        {
          Id = x.Id,
          BusinessId = x.BusinessId,
          BusinessName = x.BusinessName,
          ReviewerId = x.ReviewerId,
          ReviewerName = userDetails.ContainsKey(x.ReviewerId) ? userDetails[x.ReviewerId] : null,
          Rating = x.Rating,
          Comment = x.Comment,
          IsVerified = x.IsVerified,
          IsApproved = x.IsApproved,
          Icon = x.Icon,
          AuthUserId = x.AuthUserId,
          AuthCustomerId = x.AuthCustomerId,
          AuthUserName = x.AuthUserId.HasValue && userDetails.ContainsKey(x.AuthUserId.Value) ? userDetails[x.AuthUserId.Value] : null,
          AuthCustomerName = x.AuthUserId.HasValue && authCustomerDetails.ContainsKey(x.AuthUserId.Value) ? authCustomerDetails[x.AuthUserId.Value] : null,
          CreateUserId = x.CreateUserId,
          CreateUserName = x.CreateUserId.HasValue && userDetails.ContainsKey(x.CreateUserId.Value) ? userDetails[x.CreateUserId.Value] : null,
          UpdateUserId = x.UpdateUserId,
          UpdateUserName = x.UpdateUserId.HasValue && userDetails.ContainsKey(x.UpdateUserId.Value) ? userDetails[x.UpdateUserId.Value] : null,
          RowCreatedDate = x.RowCreatedDate,
          RowUpdatedDate = x.RowUpdatedDate,
          RowIsActive = x.RowIsActive,
          RowIsDeleted = x.RowIsDeleted
        });

        return enrichedQuery;
      }
      catch (Exception)
      {
        // Hata durumunda orijinal query'yi döndür
        return query;
      }
    }
  }
}
