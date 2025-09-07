using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.Payments.GetPaymentsById
{
	public class GetPaymentsByIdQueryRequest : BaseQueryRequest, IRequest<TransactionResultPack<GetPaymentsByIdQueryResponse>>
	{

	}
}
