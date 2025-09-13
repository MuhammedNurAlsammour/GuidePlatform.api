
using GuidePlatform.Application.Dtos.ResponseDtos;
using GuidePlatform.Domain.Enums;

namespace GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers
{
  public class JobSeekersDTO : BaseResponseDTO
  {
    public Guid BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public int Duration { get; set; } = 0;
    public bool IsSponsored { get; set; } = false;
    public Guid? ProvinceId { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
    public JobSeekerStatus Status { get; set; } = JobSeekerStatus.Pending;

    // 🎯 Sponsorlu iş arayan resmi - Sponsored job seeker image
    public string? ImageJobSeekerSponsored { get; set; }

    // 🎯 Sponsorlu iş arayan metni - Sponsored job seeker text
    public string? TextJobSeekerSponsored { get; set; }
  }
}
