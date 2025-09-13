using GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.BulkApproveJobSeekers
{
  public class BulkApproveJobSeekersCommandResponse
  {
    public List<JobSeekersDTO> UpdatedJobSeekers { get; set; } = new List<JobSeekersDTO>();
    public List<string> NotFoundIds { get; set; } = new List<string>();
    public List<string> InvalidIds { get; set; } = new List<string>();
    public int TotalRequested { get; set; }
    public int SuccessfullyUpdated { get; set; }
    public int Failed { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
  }
}
