using MediatR;
using GuidePlatform.Application.Dtos.Response;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetODataBusinessReviews
{
  /// <summary>
  /// OData BusinessReviews için Request
  /// </summary>
  public class GetODataBusinessReviewsQueryRequest : IRequest<TransactionResultPack<IQueryable<Application.Dtos.ResponseDtos.BusinessReviews.BusinessReviewsDTO>>>
  {
    // OData query options otomatik olarak EnableQuery attribute tarafından işlenir
    // Bu class sadece MediatR pattern için gerekli
  }
}
