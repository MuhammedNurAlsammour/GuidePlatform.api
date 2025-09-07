using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Subscriptions.GetAllDropboxesSubscriptions
{
	public class GetAllDropboxesSubscriptionsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesSubscriptionsQueryResponse>>
	{
	}
}
