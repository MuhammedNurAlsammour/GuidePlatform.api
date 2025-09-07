using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Pages.GetAllPages
{
  /// <summary>
  /// Pages için filtreleme ve sayfalama parametreleri
  /// </summary>
  public class GetAllPagesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllPagesQueryResponse>>
  {
    /// <summary>
    /// Yayın durumu filtresi - Published status filter
    /// </summary>
    public bool? IsPublished { get; set; }

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
