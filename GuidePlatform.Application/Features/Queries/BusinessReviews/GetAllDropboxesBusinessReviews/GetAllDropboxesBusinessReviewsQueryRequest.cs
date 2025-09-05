using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetAllDropboxesBusinessReviews
{
	public class GetAllDropboxesBusinessReviewsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesBusinessReviewsQueryResponse>>
	{
	}
}
