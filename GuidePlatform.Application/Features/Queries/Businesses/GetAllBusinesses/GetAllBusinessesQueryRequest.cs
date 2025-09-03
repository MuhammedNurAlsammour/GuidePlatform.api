using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Businesses.GetAllBusinesses
{
  public class GetAllBusinessesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessesQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor
  }
}
