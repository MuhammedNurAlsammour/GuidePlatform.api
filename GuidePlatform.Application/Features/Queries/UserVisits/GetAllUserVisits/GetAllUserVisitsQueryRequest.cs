using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.UserVisits.GetAllUserVisits
{
  public class GetAllUserVisitsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllUserVisitsQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor
  }
}
