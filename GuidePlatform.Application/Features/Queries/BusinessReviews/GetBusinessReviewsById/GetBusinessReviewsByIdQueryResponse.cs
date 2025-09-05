using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews; namespace
GuidePlatform.Application.Features.Queries.BusinessReviews.GetBusinessReviewsById
{ public class
GetBusinessReviewsByIdQueryResponse { public int TotalCount { get; set; } public
BusinessReviewsDTO
businessReviews
{ get; set; } = new(); } }