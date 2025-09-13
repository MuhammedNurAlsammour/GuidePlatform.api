using GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.JobOpportunities.GetAllJobOpportunities
{
  public class GetAllJobOpportunitiesQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<JobOpportunitiesDTO> jobOpportunities { get; set; } = new();
  }
}
