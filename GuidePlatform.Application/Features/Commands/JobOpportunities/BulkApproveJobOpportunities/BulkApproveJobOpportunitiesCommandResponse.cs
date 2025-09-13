using GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.BulkApproveJobOpportunities
{
  public class BulkApproveJobOpportunitiesCommandResponse
  {
    public List<JobOpportunitiesDTO> UpdatedJobOpportunities { get; set; } = new List<JobOpportunitiesDTO>();
    public List<string> NotFoundIds { get; set; } = new List<string>();
    public List<string> InvalidIds { get; set; } = new List<string>();
    public int TotalRequested { get; set; }
    public int SuccessfullyUpdated { get; set; }
    public int Failed { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
  }
}
