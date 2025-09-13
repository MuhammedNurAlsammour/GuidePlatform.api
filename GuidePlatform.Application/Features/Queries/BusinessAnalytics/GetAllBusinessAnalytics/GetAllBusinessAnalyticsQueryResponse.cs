using GuidePlatform.Application.Dtos.ResponseDtos.BusinessAnalytics;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetAllBusinessAnalytics
{
  public class GetAllBusinessAnalyticsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<BusinessAnalyticsDTO> businessAnalytics { get; set; } = new();
  }
}
