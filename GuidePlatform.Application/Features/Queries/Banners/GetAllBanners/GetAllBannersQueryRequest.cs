using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Banners.GetAllBanners
{
  /// <summary>
  /// Banners için filtreleme ve sayfalama parametreleri
  /// </summary>
  public class GetAllBannersQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBannersQueryResponse>>
  {
    /// <summary>
    /// il durumu filtresi - Province status filter
    /// </summary>
    public Guid? ProvinceId { get; set; } // Belirli bir şehre ait arayanlar
    /// <summary>
    /// Aktif durumu filtresi - Active status filter
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Başlık filtresi - Title filter (içerik arama)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Başlangıç tarihi filtresi - Start date filter
    /// </summary>
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }

    /// <summary>
    /// Bitiş tarihi filtresi - End date filter
    /// </summary>
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
  }
}
