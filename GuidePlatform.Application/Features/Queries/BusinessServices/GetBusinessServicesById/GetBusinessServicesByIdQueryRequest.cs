using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetBusinessServicesById
{
	public class GetBusinessServicesByIdQueryRequest : BaseQueryRequest, IRequest<TransactionResultPack<GetBusinessServicesByIdQueryResponse>>
	{

	}
}
