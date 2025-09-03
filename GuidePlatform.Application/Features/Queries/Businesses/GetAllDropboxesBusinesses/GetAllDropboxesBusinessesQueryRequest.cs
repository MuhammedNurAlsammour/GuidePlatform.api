using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Businesses.GetAllDropboxesBusinesses
{
	public class GetAllDropboxesBusinessesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesBusinessesQueryResponse>>
	{
	}
}
