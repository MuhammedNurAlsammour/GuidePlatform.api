using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.categories.GetAllDropboxesCategories
{
	public class GetAllDropboxesCategoriesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesCategoriesQueryResponse>>
	{
	}
}
