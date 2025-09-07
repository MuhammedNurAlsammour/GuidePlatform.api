using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.Pages.GetPagesById
{
	public class GetPagesByIdQueryRequest : BaseQueryRequest, IRequest<TransactionResultPack<GetPagesByIdQueryResponse>>
	{

	}
}
