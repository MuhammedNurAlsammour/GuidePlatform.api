using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Articles.GetAllArticles
{
  /// <summary>
  /// Articles için filtreleme ve sayfalama parametreleri
  /// </summary>
  public class GetAllArticlesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllArticlesQueryResponse>>
  {
    /// <summary>
    /// Yazar kimliği filtresi - Author ID filter
    /// </summary>
    public Guid? AuthorId { get; set; }

    /// <summary>
    /// Kategori kimliği filtresi - Category ID filter
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Yayın durumu filtresi - Published status filter
    /// </summary>
    public bool? IsPublished { get; set; }

    /// <summary>
    /// Öne çıkan makale filtresi - Featured article filter
    /// </summary>
    public bool? IsFeatured { get; set; }

    /// <summary>
    /// Başlık filtresi - Title filter (içerik arama)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Slug filtresi - Slug filter
    /// </summary>
    public string? Slug { get; set; }
  }
}
