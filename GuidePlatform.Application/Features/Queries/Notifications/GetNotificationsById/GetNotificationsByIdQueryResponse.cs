using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.Notifications; namespace
GuidePlatform.Application.Features.Queries.Notifications.GetNotificationsById
{ public class
GetNotificationsByIdQueryResponse { public int TotalCount { get; set; } public
NotificationsDTO
notifications
{ get; set; } = new(); } }