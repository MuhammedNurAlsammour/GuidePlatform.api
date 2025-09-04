using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.UserVisits.GetAllDropboxesUserVisits
{
	public class GetAllDropboxesUserVisitsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesUserVisitsQueryResponse>>
	{
	}
}
