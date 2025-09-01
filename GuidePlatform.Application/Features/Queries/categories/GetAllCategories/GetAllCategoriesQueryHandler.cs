using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.categories;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.categories.GetAllCategories
{
  public class GetAllCategoriesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllCategoriesQueryRequest, TransactionResultPack<GetAllCategoriesQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllCategoriesQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllCategoriesQueryResponse>> Handle(GetAllCategoriesQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.categories
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var categoriess = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // Auth kullanıcı bilgilerini service ile al
        var authUserIds = categoriess.Select(categories => categories.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

        var categoriesDetails = new List<categoriesDTO>();  // 🎯 GetAllCategoriesDTO listesi oluştur

        foreach (var categories in categoriess)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (categories.AuthUserId.HasValue && authUserDetails.ContainsKey(categories.AuthUserId.Value))
          {
            var userDetail = authUserDetails[categories.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
          }

          var categoriesDetail = new categoriesDTO
          {
            Id = categories.Id,
            Name = categories.Name,
            Description = categories.Description,
            ParentId = categories.ParentId,
            ParentName = parentName,
            Icon = categories.Icon,
            SortOrder = categories.SortOrder,
            ChildrenCount = childrenCount,
            FullPath = await GetCategoryFullPath(categories, cancellationToken),
            AuthUserId = categories.AuthUserId,
            AuthCustomerId = categories.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = categories.CreateUserId,
            UpdateUserId = categories.UpdateUserId,
            RowCreatedDate = categories.RowCreatedDate,
            RowUpdatedDate = categories.RowUpdatedDate,
            RowIsActive = categories.RowIsActive,
            RowIsDeleted = categories.RowIsDeleted
          };

          categoriesDetails.Add(categoriesDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllCategoriesQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllCategoriesQueryResponse
            {
              TotalCount = totalCount,
              categories = categoriesDetails  // 🎯 GetAllCategoriesDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "categories başarıyla getirildi.",
            $"categoriess.Count categories başarıyla getirildi."  // 🎯 categories sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllCategoriesQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "categories getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}
