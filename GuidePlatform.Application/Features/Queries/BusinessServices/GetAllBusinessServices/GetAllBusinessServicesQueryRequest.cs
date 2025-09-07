using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetAllBusinessServices
{
  public class GetAllBusinessServicesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessServicesQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🏢 İşletme bilgileri - Business information
    public Guid? BusinessId { get; set; }                // İşletme ID - Business ID

    // 💼 Hizmet bilgileri - Service information
    public string? ServiceName { get; set; }             // Hizmet adı - Service name
    public string? ServiceDescription { get; set; }      // Hizmet açıklaması - Service description
    public decimal? MinPrice { get; set; }               // Minimum fiyat - Minimum price
    public decimal? MaxPrice { get; set; }               // Maksimum fiyat - Maximum price
    public string? Currency { get; set; }                // Para birimi - Currency
    public bool? IsAvailable { get; set; }               // Mevcut mu - Is available
    public string? Icon { get; set; }                    // İkon - Icon
  }
}
