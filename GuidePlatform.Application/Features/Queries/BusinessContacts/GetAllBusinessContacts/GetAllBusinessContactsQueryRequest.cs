using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessContacts.GetAllBusinessContacts
{
  public class GetAllBusinessContactsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessContactsQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🏢 İşletme bilgileri - Business information
    public Guid? BusinessId { get; set; }                // İşletme ID - Business ID

    // 📞 İletişim bilgileri - Contact information
    public string? ContactType { get; set; }              // İletişim türü - Contact type
    public string? ContactValue { get; set; }             // İletişim değeri - Contact value
    public bool? IsPrimary { get; set; }                 // Ana iletişim mi - Is primary contact
  }
}
