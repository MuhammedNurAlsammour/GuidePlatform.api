using GuidePlatform.Domain.Entities.Common;
using GuidePlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class JobSeekersViewModel : BaseEntity
  {
    public Guid? BusinessId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public int Duration { get; set; } = 0;
    public bool IsSponsored { get; set; } = false;
    public Guid? ProvinceId { get; set; }
    public JobSeekerStatus Status { get; set; } = JobSeekerStatus.Pending;
  }
}