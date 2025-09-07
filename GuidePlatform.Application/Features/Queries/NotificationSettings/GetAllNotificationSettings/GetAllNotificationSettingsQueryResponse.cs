using GuidePlatform.Application.Dtos.ResponseDtos.NotificationSettings;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.NotificationSettings.GetAllNotificationSettings
{
  public class GetAllNotificationSettingsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<NotificationSettingsDTO> notificationSettings { get; set; } = new();
  }
}
