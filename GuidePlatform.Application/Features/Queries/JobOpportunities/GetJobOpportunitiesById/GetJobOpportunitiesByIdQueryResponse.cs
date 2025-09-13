using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities; namespace
GuidePlatform.Application.Features.Queries.JobOpportunities.GetJobOpportunitiesById
{ public class
GetJobOpportunitiesByIdQueryResponse { public int TotalCount { get; set; } public
JobOpportunitiesDTO
jobOpportunities
{ get; set; } = new(); } }