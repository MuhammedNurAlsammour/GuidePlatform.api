using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.SearchLogs.GetSearchLogsById
{
	public class GetSearchLogsByIdQueryRequest : BaseQueryRequest, IRequest<TransactionResultPack<GetSearchLogsByIdQueryResponse>>
	{

	}
}
