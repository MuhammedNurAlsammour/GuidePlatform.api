using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.SearchLogs.GetAllDropboxesSearchLogs
{
	public class GetAllDropboxesSearchLogsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesSearchLogsQueryResponse>>
	{
	}
}
