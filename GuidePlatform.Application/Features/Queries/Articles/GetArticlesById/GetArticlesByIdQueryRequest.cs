using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.Articles.GetArticlesById
{
	public class GetArticlesByIdQueryRequest : BaseQueryRequest, IRequest<TransactionResultPack<GetArticlesByIdQueryResponse>>
	{

	}
}
