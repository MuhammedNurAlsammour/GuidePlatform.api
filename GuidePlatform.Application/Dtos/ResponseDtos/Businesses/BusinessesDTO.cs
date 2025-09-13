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
    public string? SubDescription { get; set; } // وصف فرعي أو مختصر
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

    // 🎯 Ana iletişim bilgileri - Primary contact information
    public int? PrimaryContactType1 { get; set; }      // 1:WhatsApp, 2:Phone, 3:Mobile, 4:Email, 5:Facebook, 6:Instagram, 7:Telegram, 8:Website
    public string? PrimaryContactValue1 { get; set; }  // Ana iletişim değeri 1 - Primary contact value 1
    public int? PrimaryContactType2 { get; set; }      // 1:WhatsApp, 2:Phone, 3:Mobile, 4:Email, 5:Facebook, 6:Instagram, 7:Telegram, 8:Website
    public string? PrimaryContactValue2 { get; set; }  // Ana iletişim değeri 2 - Primary contact value 2

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

    // 📸 Resim bilgileri - Image information
    public string? MainPhoto { get; set; }
    public List<string> BannerPhotos { get; set; } = new List<string>();
  }
}
