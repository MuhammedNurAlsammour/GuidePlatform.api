using GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.UpdateJobSeekerStatus
{
  public class UpdateJobSeekerStatusCommandResponse
  {
    public JobSeekersDTO JobSeeker { get; set; } = new();
    public string PreviousStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
  }
}
