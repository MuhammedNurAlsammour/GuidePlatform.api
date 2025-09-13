

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.Banners
{
  public class BannersDTO : BaseResponseDTO
  {
    // Banner ile ilgili temel bilgiler - Basic banner information
    public string Title { get; set; } = string.Empty;
    // 📍 Konum bilgileri - Location information
    public Guid? ProvinceId { get; set; }
    public string? Description { get; set; }
    public byte[]? Photo { get; set; } // Eski sistem için korunuyor
    public byte[]? Thumbnail { get; set; } // Eski sistem için korunuyor
    public string? PhotoUrl { get; set; } // Yeni sistem: Fotoğraf URL'si
    public string? ThumbnailUrl { get; set; } // Yeni sistem: Küçük resim URL'si
    public string? PhotoContentType { get; set; }
    public string? LinkUrl { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int OrderIndex { get; set; } = 0;
    public string Icon { get; set; } = "image";
  }
}
