using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Articles;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Articles.GetArticlesById
{
  // Bu handler, bir articles ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetArticlesByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetArticlesByIdQueryRequest, TransactionResultPack<GetArticlesByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetArticlesByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetArticlesByIdQueryResponse>> Handle(GetArticlesByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetArticlesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "articles ID'si belirtilmedi.",
              "articles ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var articlesId = request.GetIdAsGuid();
        if (!articlesId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetArticlesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz articles ID formatı.",
              $"Geçersiz articles ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.articles
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == articlesId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve articles çekiliyor
        var articles = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == articlesId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (articles == null)
        {
          // articles bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetArticlesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / articles Bulunamadı",
              "Belirtilen ID'ye sahip articles bulunamadı.",
              $"ID '{request.Id}' ile eşleşen articles bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (articles.AuthUserId.HasValue)
          allUserIds.Add(articles.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (articles.CreateUserId.HasValue)
          allUserIds.Add(articles.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (articles.UpdateUserId.HasValue)
          allUserIds.Add(articles.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (articles.AuthUserId.HasValue && allUserDetails.ContainsKey(articles.AuthUserId.Value))
        {
          var userDetail = allUserDetails[articles.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
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

        // articles detay DTO'su oluşturuluyor
        var articlesDetail = new ArticlesDTO
        {
          Id = articles.Id,
          AuthUserId = articles.AuthUserId,
          AuthCustomerId = articles.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = articles.CreateUserId,
          UpdateUserId = articles.UpdateUserId,
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

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetArticlesByIdQueryResponse>(
            new GetArticlesByIdQueryResponse
            {
              articles = articlesDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "articles başarıyla getirildi.",
            $"articles Id: {articles.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetArticlesByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "articles getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

