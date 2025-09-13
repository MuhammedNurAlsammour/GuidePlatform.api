using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.BusinessAnalytics; namespace
GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetBusinessAnalyticsById
{ public class
GetBusinessAnalyticsByIdQueryResponse { public int TotalCount { get; set; } public
BusinessAnalyticsDTO
businessAnalytics
{ get; set; } = new(); } }