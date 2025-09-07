using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.NotificationSettings; namespace
GuidePlatform.Application.Features.Queries.NotificationSettings.GetNotificationSettingsById
{ public class
GetNotificationSettingsByIdQueryResponse { public int TotalCount { get; set; } public
NotificationSettingsDTO
notificationSettings
{ get; set; } = new(); } }