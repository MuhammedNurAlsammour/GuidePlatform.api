using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.Businesses
{
  public class BusinessesDTO : BaseResponseDTO
  {
    // 🏢 Temel iş bilgileri - Basic business information
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }

    // 📍 Konum bilgileri - Location information
    public Guid? ProvinceId { get; set; }
    public Guid? CountriesId { get; set; }
    public Guid? DistrictId { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // 📞 İletişim bilgileri - Contact information
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? WhatsApp { get; set; }
    public string? Telegram { get; set; }

    // ⭐ Değerlendirme ve istatistikler - Rating and statistics
    public decimal Rating { get; set; } = 0.00m;
    public int TotalReviews { get; set; } = 0;
    public int ViewCount { get; set; } = 0;

    // 💼 İş özellikleri - Business features
    public int SubscriptionType { get; set; } = 0;
    public bool IsVerified { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public string? WorkingHours { get; set; }
    public string Icon { get; set; } = "business";

    // 👤 Sahiplik bilgileri - Ownership information
    public Guid? OwnerId { get; set; }
  }
}
