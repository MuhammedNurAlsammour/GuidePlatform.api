using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Payments.GetAllDropboxesPayments
{
	public class GetAllDropboxesPaymentsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesPaymentsQueryResponse>>
	{
	}
}
