using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.JobOpportunities.GetAllDropboxesJobOpportunities
{
	public class GetAllDropboxesJobOpportunitiesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesJobOpportunitiesQueryResponse>>
	{
	}
}
