using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Articles.GetAllDropboxesArticles
{
	public class GetAllDropboxesArticlesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesArticlesQueryResponse>>
	{
	}
}
