using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.Announcements.CreateAnnouncements
{
  public class CreateAnnouncementsCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
