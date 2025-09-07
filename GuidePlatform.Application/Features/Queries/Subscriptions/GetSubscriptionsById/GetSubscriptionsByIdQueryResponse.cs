using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.Subscriptions; namespace
GuidePlatform.Application.Features.Queries.Subscriptions.GetSubscriptionsById
{ public class
GetSubscriptionsByIdQueryResponse { public int TotalCount { get; set; } public
SubscriptionsDTO
subscriptions
{ get; set; } = new(); } }