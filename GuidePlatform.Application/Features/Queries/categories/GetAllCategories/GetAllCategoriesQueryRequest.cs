using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.categories.GetAllCategories
{
  public class GetAllCategoriesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllCategoriesQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor
  }
}
