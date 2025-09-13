using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Announcements.GetAllAnnouncements
{
  /// <summary>
  /// Announcements için filtreleme ve sayfalama parametreleri
  /// </summary>
  public class GetAllAnnouncementsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllAnnouncementsQueryResponse>>
  {
    /// <summary>
    /// Yayın durumu filtresi - Published status filter
    /// </summary>
    public bool? IsPublished { get; set; }

    /// <summary>
    /// Öncelik filtresi - Priority filter
    /// 1: Düşük, 2: Orta, 3: Yüksek, 4: Kritik
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// Başlık filtresi - Title filter (içerik arama)
    /// </summary>
    public string? Title { get; set; }
  }
}
