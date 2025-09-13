using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.Announcements; namespace
GuidePlatform.Application.Features.Queries.Announcements.GetAnnouncementsById
{ public class
GetAnnouncementsByIdQueryResponse { public int TotalCount { get; set; } public
AnnouncementsDTO
announcements
{ get; set; } = new(); } }