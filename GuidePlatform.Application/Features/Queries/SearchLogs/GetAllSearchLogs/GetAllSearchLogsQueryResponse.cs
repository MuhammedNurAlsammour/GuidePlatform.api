using GuidePlatform.Application.Dtos.ResponseDtos.SearchLogs;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.SearchLogs.GetAllSearchLogs
{
  public class GetAllSearchLogsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<SearchLogsDTO> searchLogs { get; set; } = new();
  }
}
