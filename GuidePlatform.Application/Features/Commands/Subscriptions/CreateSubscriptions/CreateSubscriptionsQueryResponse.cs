using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.Subscriptions.CreateSubscriptions
{
  public class CreateSubscriptionsCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
