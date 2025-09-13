using GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.JobSeekers.GetAllJobSeekers
{
  public class GetAllJobSeekersQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<JobSeekersDTO> jobSeekers { get; set; } = new();
  }
}
