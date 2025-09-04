using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.UserFavorites.GetAllDropboxesUserFavorites
{
	public class GetAllDropboxesUserFavoritesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesUserFavoritesQueryResponse>>
	{
	}
}
