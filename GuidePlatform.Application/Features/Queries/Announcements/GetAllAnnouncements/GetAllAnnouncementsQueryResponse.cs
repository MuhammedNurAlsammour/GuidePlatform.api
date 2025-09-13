using GuidePlatform.Application.Dtos.ResponseDtos.Announcements;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Announcements.GetAllAnnouncements
{
  public class GetAllAnnouncementsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<AnnouncementsDTO> announcements { get; set; } = new();
  }
}
