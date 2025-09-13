using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.Domain.Enums;

namespace GuidePlatform.Application.Features.Queries.JobSeekers.GetAllJobSeekers
{
  public class GetAllJobSeekersQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllJobSeekersQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🎯 İş Arayan Filtreleri - Job Seeker Filters
    public Guid? BusinessId { get; set; } // Belirli bir işletmeye ait arayanlar
    public string? FullName { get; set; } // Tam isim arama
    public string? Description { get; set; } // Açıklama arama
    public string? Phone { get; set; } // Telefon numarası arama
    public int? MinDuration { get; set; } // Minimum süre
    public int? MaxDuration { get; set; } // Maksimum süre
    public bool? IsSponsored { get; set; } // Sponsorlu arayanlar
    public Guid? ProvinceId { get; set; } // Belirli bir şehre ait arayanlar
    public JobSeekerStatus? Status { get; set; } // Durum filtresi

    // 🎯 Tarih Filtreleri - Date Filters
    public DateTime? CreatedDateFrom { get; set; } // Oluşturulma tarihi başlangıç
    public DateTime? CreatedDateTo { get; set; } // Oluşturulma tarihi bitiş
    public DateTime? UpdatedDateFrom { get; set; } // Güncellenme tarihi başlangıç
    public DateTime? UpdatedDateTo { get; set; } // Güncellenme tarihi bitiş

    // 🎯 Kullanıcı Filtreleri - User Filters
    public Guid? AuthUserId { get; set; } // Belirli kullanıcıya ait arayanlar
    public Guid? AuthCustomerId { get; set; } // Belirli müşteriye ait arayanlar
    public Guid? CreateUserId { get; set; } // Belirli kullanıcı tarafından oluşturulan arayanlar
    public Guid? UpdateUserId { get; set; } // Belirli kullanıcı tarafından güncellenen arayanlar
  }
}
