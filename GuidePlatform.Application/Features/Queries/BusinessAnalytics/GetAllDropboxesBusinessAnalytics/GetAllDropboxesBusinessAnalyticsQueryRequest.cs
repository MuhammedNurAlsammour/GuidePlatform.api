using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetAllDropboxesBusinessAnalytics
{
	public class GetAllDropboxesBusinessAnalyticsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesBusinessAnalyticsQueryResponse>>
	{
	}
}
