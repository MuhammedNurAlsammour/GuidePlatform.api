using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers; namespace
GuidePlatform.Application.Features.Queries.JobSeekers.GetJobSeekersById
{ public class
GetJobSeekersByIdQueryResponse { public int TotalCount { get; set; } public
JobSeekersDTO
jobSeekers
{ get; set; } = new(); } }