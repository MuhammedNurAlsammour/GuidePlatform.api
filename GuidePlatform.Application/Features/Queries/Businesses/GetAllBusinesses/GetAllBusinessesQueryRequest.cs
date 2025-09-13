using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Businesses.GetAllBusinesses
{
  public class GetAllBusinessesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessesQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🏢 Temel iş bilgileri - Basic business information
    public string? Name { get; set; }                    // İşletme adı - Business name
    public string? Description { get; set; }             // İşletme açıklaması - Business description
    public string? SubDescription { get; set; }          // وصف فرعي - Sub description
    public Guid? CategoryId { get; set; }                // Kategori ID - Category ID
    public Guid? SubCategoryId { get; set; }             // Alt kategori ID - Sub category ID

    // 📍 Konum bilgileri - Location information
    public Guid? ProvinceId { get; set; }                // İl ID - Province ID
    public Guid? CountriesId { get; set; }               // Ülke ID - Country ID
    public Guid? DistrictId { get; set; }                // İlçe ID - District ID
    public string? Address { get; set; }                 // Adres - Address

    // 📞 İletişim bilgileri - Contact information
    public string? Phone { get; set; }                   // Telefon - Phone
    public string? Mobile { get; set; }                  // Cep telefonu - Mobile
    public string? Email { get; set; }                   // E-posta - Email
    public string? Website { get; set; }                 // Web sitesi - Website
    public string? FacebookUrl { get; set; }             // Facebook URL
    public string? InstagramUrl { get; set; }            // Instagram URL
    public string? WhatsApp { get; set; }                // WhatsApp
    public string? Telegram { get; set; }                // Telegram

    // 🎯 Ana iletişim bilgileri - Primary contact information
    public int? PrimaryContactType1 { get; set; }        // Ana iletişim türü 1 - Primary contact type 1
    public string? PrimaryContactValue1 { get; set; }    // Ana iletişim değeri 1 - Primary contact value 1
    public int? PrimaryContactType2 { get; set; }        // Ana iletişim türü 2 - Primary contact type 2
    public string? PrimaryContactValue2 { get; set; }    // Ana iletişim değeri 2 - Primary contact value 2

    // ⭐ Değerlendirme ve istatistikler - Rating and statistics
    public decimal? MinRating { get; set; }              // Minimum puan - Minimum rating
    public decimal? MaxRating { get; set; }              // Maksimum puan - Maximum rating
    public int? MinTotalReviews { get; set; }            // Minimum yorum sayısı - Minimum total reviews
    public int? MaxTotalReviews { get; set; }            // Maksimum yorum sayısı - Maximum total reviews
    public int? MinViewCount { get; set; }               // Minimum görüntülenme - Minimum view count
    public int? MaxViewCount { get; set; }               // Maksimum görüntülenme - Maximum view count

    // 💼 İş özellikleri - Business features
    public int? SubscriptionType { get; set; }           // Abonelik türü - Subscription type
    public bool? IsVerified { get; set; }                // Doğrulanmış mı - Is verified
    public bool? IsFeatured { get; set; }                // Öne çıkarılmış mı - Is featured
    public string? WorkingHours { get; set; }            // Çalışma saatleri - Working hours
    public string? Icon { get; set; }                    // İkon - Icon

    // 👤 Sahiplik bilgileri - Ownership information
    public Guid? OwnerId { get; set; }                   // Sahip ID - Owner ID
    public Guid? AuthUserId { get; set; }                // Auth kullanıcı ID - Auth user ID
    public Guid? AuthCustomerId { get; set; }            // Auth müşteri ID - Auth customer ID

    // 📅 Tarih filtreleri - Date filters
    public DateTime? CreatedDateFrom { get; set; }       // Oluşturulma tarihi başlangıç - Created date from
    public DateTime? CreatedDateTo { get; set; }         // Oluşturulma tarihi bitiş - Created date to
    public DateTime? UpdatedDateFrom { get; set; }       // Güncellenme tarihi başlangıç - Updated date from
    public DateTime? UpdatedDateTo { get; set; }         // Güncellenme tarihi bitiş - Updated date to

    // 🔍 Genel arama - General search
    public string? SearchTerm { get; set; }              // Genel arama terimi - General search term
  }
}
