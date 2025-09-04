using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetAllBusinessServices
{
  public class GetAllBusinessServicesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessServicesQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor
  }
}
