using GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.UpdateJobOpportunityStatus
{
  public class UpdateJobOpportunityStatusCommandResponse
  {
    public JobOpportunitiesDTO JobOpportunity { get; set; } = new();
    public string PreviousStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
  }
}
