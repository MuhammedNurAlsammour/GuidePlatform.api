using GuidePlatform.Application.Dtos.ResponseDtos.Notifications;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Notifications.GetAllNotifications
{
  public class GetAllNotificationsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<NotificationsDTO> notifications { get; set; } = new();
  }
}
