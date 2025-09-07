using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.Notifications.CreateNotifications
{
  public class CreateNotificationsCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
