using GuidePlatform.Application.Dtos.ResponseDtos.Subscriptions;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Subscriptions.GetAllSubscriptions
{
  public class GetAllSubscriptionsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<SubscriptionsDTO> subscriptions { get; set; } = new();
  }
}
