using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.SearchLogs.GetAllSearchLogs
{
  public class GetAllSearchLogsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllSearchLogsQueryResponse>>
  {
    // Search specific filters - Arama özel filtreleri
    public string? SearchTerm { get; set; }
    public string? SearchFilters { get; set; }
    public int? ResultsCountFrom { get; set; }
    public int? ResultsCountTo { get; set; }
    public DateTime? SearchDateFrom { get; set; }
    public DateTime? SearchDateTo { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Icon { get; set; }
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
    public DateTime? UpdatedDateFrom { get; set; }
    public DateTime? UpdatedDateTo { get; set; }
  }
}
