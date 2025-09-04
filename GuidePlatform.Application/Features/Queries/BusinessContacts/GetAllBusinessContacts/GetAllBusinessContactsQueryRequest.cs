using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessContacts.GetAllBusinessContacts
{
  public class GetAllBusinessContactsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessContactsQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor
  }
}
