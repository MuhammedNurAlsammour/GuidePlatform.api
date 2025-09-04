using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetAllDropboxesBusinessServices
{
	public class GetAllDropboxesBusinessServicesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesBusinessServicesQueryResponse>>
	{
	}
}
