using GuidePlatform.Application.Dtos.ResponseDtos;
using GuidePlatform.Domain.Enums;

namespace GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities
{
  public class JobOpportunitiesDTO : BaseResponseDTO
  {
    public Guid BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int Duration { get; set; } = 0;
    public bool IsSponsored { get; set; } = false;
    public Guid? ProvinceId { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
    public JobOpportunityStatus Status { get; set; } = JobOpportunityStatus.Pending;

    // 🎯 Sponsorlu iş ilanı resmi - Sponsored job opportunity image
    public string? ImageJobOpportunitieSponsored { get; set; }

    // 🎯 Sponsorlu iş ilanı metni - Sponsored job opportunity text
    public string? TextJobOpportunitieSponsored { get; set; }
  }
}
