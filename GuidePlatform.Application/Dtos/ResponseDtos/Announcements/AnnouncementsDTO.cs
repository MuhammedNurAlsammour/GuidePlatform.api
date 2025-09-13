

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.Announcements
{
  public class AnnouncementsDTO : BaseResponseDTO
  {
    // Duyuru ile ilgili temel bilgiler - Basic announcement information
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedDate { get; set; }
    public string Icon { get; set; } = "announcement";
  }
}
