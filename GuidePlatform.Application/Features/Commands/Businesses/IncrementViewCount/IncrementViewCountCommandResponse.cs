using GuidePlatform.Application.Dtos.Base;
using GuidePlatform.Application.Dtos.Response;

namespace GuidePlatform.Application.Features.Commands.Businesses.IncrementViewCount
{
  public class IncrementViewCountCommandResponse : BaseResponseDto
  {
    public Guid BusinessId { get; set; }
    public int NewViewCount { get; set; }
    public DateTime ViewedAt { get; set; }
  }
}
