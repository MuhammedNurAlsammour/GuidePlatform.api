using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.UserVisits.GetAllUserVisits
{
  public class GetAllUserVisitsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllUserVisitsQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🏢 İşletme bilgileri - Business information
    public Guid? BusinessId { get; set; }                // İşletme ID - Business ID

    // 📅 Ziyaret bilgileri - Visit information
    public DateTime? VisitDate { get; set; }             // Ziyaret tarihi - Visit date
    public string? VisitType { get; set; }               // Ziyaret türü - Visit type
    public string? Icon { get; set; }                    // İkon - Icon
  }
}
