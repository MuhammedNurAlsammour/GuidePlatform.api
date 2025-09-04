using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetAllBusinessImages
{
  public class GetAllBusinessImagesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessImagesQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor
  }
}
