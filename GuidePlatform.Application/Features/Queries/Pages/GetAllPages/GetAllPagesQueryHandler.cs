using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Pages;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Pages.GetAllPages
{
  public class GetAllPagesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllPagesQueryRequest, TransactionResultPack<GetAllPagesQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllPagesQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllPagesQueryResponse>> Handle(GetAllPagesQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.pages
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyPagesFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var pagess = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = pagess.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = pagess.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = pagess.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var pagesDetails = new List<PagesDTO>();  // 🎯 pagesDTO listesi oluştur

        foreach (var pages in pagess)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (pages.AuthUserId.HasValue && allUserDetails.ContainsKey(pages.AuthUserId.Value))
          {
            var userDetail = allUserDetails[pages.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
          }

          // 🎯 Create/Update kullanıcı bilgilerini al
          string? createUserName = null;
          string? updateUserName = null;

          if (pages.CreateUserId.HasValue && allUserDetails.ContainsKey(pages.CreateUserId.Value))
          {
            var createUserDetail = allUserDetails[pages.CreateUserId.Value];
            createUserName = createUserDetail.AuthUserName;
          }

          if (pages.UpdateUserId.HasValue && allUserDetails.ContainsKey(pages.UpdateUserId.Value))
          {
            var updateUserDetail = allUserDetails[pages.UpdateUserId.Value];
            updateUserName = updateUserDetail.AuthUserName;
          }

          var pagesDetail = new PagesDTO
          {
            Id = pages.Id,
            AuthUserId = pages.AuthUserId,
            AuthCustomerId = pages.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = pages.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = pages.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = pages.RowCreatedDate,
            RowUpdatedDate = pages.RowUpdatedDate,
            RowIsActive = pages.RowIsActive,
            RowIsDeleted = pages.RowIsDeleted,
            // Sayfa özel alanları - Page specific fields
            Title = pages.Title,
            Slug = pages.Slug,
            Content = pages.Content,
            MetaDescription = pages.MetaDescription,
            MetaKeywords = pages.MetaKeywords,
            IsPublished = pages.IsPublished,
            PublishedDate = pages.PublishedDate,
            Icon = pages.Icon
          };

          pagesDetails.Add(pagesDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllPagesQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllPagesQueryResponse
            {
              TotalCount = totalCount,
              pages = pagesDetails  // 🎯 pagesDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "pages başarıyla getirildi.",
            $"pagess.Count pages başarıyla getirildi."  // 🎯 pages sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllPagesQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "pages getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı Pages filtrelerini uygular - Applies Pages filters
    /// </summary>
    private IQueryable<PagesViewModel> ApplyPagesFilters(
        IQueryable<PagesViewModel> query,
        GetAllPagesQueryRequest request)
    {
      // 🔍 Yayın durumu filtresi - Published status filter
      if (request.IsPublished.HasValue)
      {
        query = query.Where(x => x.IsPublished == request.IsPublished.Value);
      }

      // 🔍 Başlık filtresi - Title filter (içerik arama)
      if (!string.IsNullOrWhiteSpace(request.Title))
      {
        query = query.Where(x => x.Title.Contains(request.Title.Trim()));
      }

      // 🔍 Slug filtresi - Slug filter
      if (!string.IsNullOrWhiteSpace(request.Slug))
      {
        query = query.Where(x => x.Slug == request.Slug.Trim());
      }

      return query;
    }
  }
}
