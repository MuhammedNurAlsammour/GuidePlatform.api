using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Pages.GetAllDropboxesPages
{
	public class GetAllDropboxesPagesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesPagesQueryResponse>>
	{
	}
}
