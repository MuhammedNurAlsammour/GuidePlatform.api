using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class BannersViewModel : BaseEntity
  {
    // Banner ile ilgili temel bilgiler - Basic banner information
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public byte[]? Photo { get; set; }
    public byte[]? Thumbnail { get; set; }
    public string? PhotoContentType { get; set; }
    public string? LinkUrl { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int OrderIndex { get; set; } = 0;
    public string Icon { get; set; } = "image";
  }
}