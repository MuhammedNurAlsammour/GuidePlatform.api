using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.Domain.Enums;

namespace GuidePlatform.Application.Features.Queries.JobOpportunities.GetAllJobOpportunities
{
  public class GetAllJobOpportunitiesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllJobOpportunitiesQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🎯 İş İlanı Filtreleri - Job Opportunity Filters
    public Guid? BusinessId { get; set; } // Belirli bir işletmeye ait ilanlar
    public string? ImageJobOpportunitieSponsored { get; set; } // Resimli ilanlar
    public string? TextJobOpportunitieSponsored { get; set; } // Metinsel ilanlar
    public string? Title { get; set; } // Başlık arama
    public string? Description { get; set; } // Açıklama arama
    public string? Phone { get; set; } // Telefon numarası arama
    public int? MinDuration { get; set; } // Minimum süre
    public int? MaxDuration { get; set; } // Maksimum süre
    public bool? IsSponsored { get; set; } // Sponsorlu ilanlar
    public Guid? ProvinceId { get; set; } // Belirli bir şehre ait ilanlar
    public JobOpportunityStatus? Status { get; set; } // Durum filtresi

    // 🎯 Tarih Filtreleri - Date Filters
    public DateTime? CreatedDateFrom { get; set; } // Oluşturulma tarihi başlangıç
    public DateTime? CreatedDateTo { get; set; } // Oluşturulma tarihi bitiş
    public DateTime? UpdatedDateFrom { get; set; } // Güncellenme tarihi başlangıç
    public DateTime? UpdatedDateTo { get; set; } // Güncellenme tarihi bitiş

    // 🎯 Kullanıcı Filtreleri - User Filters
    public Guid? AuthUserId { get; set; } // Belirli kullanıcıya ait ilanlar
    public Guid? AuthCustomerId { get; set; } // Belirli müşteriye ait ilanlar
    public Guid? CreateUserId { get; set; } // Belirli kullanıcı tarafından oluşturulan ilanlar
    public Guid? UpdateUserId { get; set; } // Belirli kullanıcı tarafından güncellenen ilanlar
  }
}
