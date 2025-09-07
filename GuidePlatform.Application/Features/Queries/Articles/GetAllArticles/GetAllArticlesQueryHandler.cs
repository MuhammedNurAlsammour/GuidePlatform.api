using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Articles;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Articles.GetAllArticles
{
  public class GetAllArticlesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllArticlesQueryRequest, TransactionResultPack<GetAllArticlesQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllArticlesQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllArticlesQueryResponse>> Handle(GetAllArticlesQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.articles
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyArticlesFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var articless = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = articless.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = articless.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = articless.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var articlesDetails = new List<ArticlesDTO>();  // 🎯 articlesDTO listesi oluştur

        foreach (var articles in articless)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (articles.AuthUserId.HasValue && allUserDetails.ContainsKey(articles.AuthUserId.Value))
          {
            var userDetail = allUserDetails[articles.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
          }

          // 🎯 Create/Update kullanıcı bilgilerini al
          string? createUserName = null;
          string? updateUserName = null;

          if (articles.CreateUserId.HasValue && allUserDetails.ContainsKey(articles.CreateUserId.Value))
          {
            var createUserDetail = allUserDetails[articles.CreateUserId.Value];
            createUserName = createUserDetail.AuthUserName;
          }

          if (articles.UpdateUserId.HasValue && allUserDetails.ContainsKey(articles.UpdateUserId.Value))
          {
            var updateUserDetail = allUserDetails[articles.UpdateUserId.Value];
            updateUserName = updateUserDetail.AuthUserName;
          }

          var articlesDetail = new ArticlesDTO
          {
            Id = articles.Id,
            AuthUserId = articles.AuthUserId,
            AuthCustomerId = articles.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = articles.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = articles.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = articles.RowCreatedDate,
            RowUpdatedDate = articles.RowUpdatedDate,
            RowIsActive = articles.RowIsActive,
            RowIsDeleted = articles.RowIsDeleted,
            // Makale özel alanları - Article specific fields
            Title = articles.Title,
            Content = articles.Content,
            Excerpt = articles.Excerpt,
            Photo = articles.Photo,
            Thumbnail = articles.Thumbnail,
            PhotoContentType = articles.PhotoContentType,
            AuthorId = articles.AuthorId,
            CategoryId = articles.CategoryId,
            IsFeatured = articles.IsFeatured,
            IsPublished = articles.IsPublished,
            PublishedAt = articles.PublishedAt,
            ViewCount = articles.ViewCount,
            SeoTitle = articles.SeoTitle,
            SeoDescription = articles.SeoDescription,
            Slug = articles.Slug,
            Icon = articles.Icon
          };

          articlesDetails.Add(articlesDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllArticlesQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllArticlesQueryResponse
            {
              TotalCount = totalCount,
              articles = articlesDetails  // 🎯 articlesDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "articles başarıyla getirildi.",
            $"articless.Count articles başarıyla getirildi."  // 🎯 articles sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllArticlesQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "articles getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı Articles filtrelerini uygular - Applies Articles filters
    /// </summary>
    private IQueryable<ArticlesViewModel> ApplyArticlesFilters(
        IQueryable<ArticlesViewModel> query,
        GetAllArticlesQueryRequest request)
    {
      // 🔍 Yazar kimliği filtresi - Author ID filter
      if (request.AuthorId.HasValue)
      {
        query = query.Where(x => x.AuthorId == request.AuthorId.Value);
      }

      // 🔍 Kategori kimliği filtresi - Category ID filter
      if (request.CategoryId.HasValue)
      {
        query = query.Where(x => x.CategoryId == request.CategoryId.Value);
      }

      // 🔍 Yayın durumu filtresi - Published status filter
      if (request.IsPublished.HasValue)
      {
        query = query.Where(x => x.IsPublished == request.IsPublished.Value);
      }

      // 🔍 Öne çıkan makale filtresi - Featured article filter
      if (request.IsFeatured.HasValue)
      {
        query = query.Where(x => x.IsFeatured == request.IsFeatured.Value);
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
