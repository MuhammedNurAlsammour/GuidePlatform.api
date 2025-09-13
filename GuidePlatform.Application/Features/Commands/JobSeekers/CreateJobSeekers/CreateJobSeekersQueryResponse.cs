using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.CreateJobSeekers
{
  public class CreateJobSeekersCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
