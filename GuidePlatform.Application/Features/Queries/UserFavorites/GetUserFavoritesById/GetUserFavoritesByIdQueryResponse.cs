using E = GuidePlatform.Domain.Entities;
using GuidePlatform.Application.Dtos.ResponseDtos;	
using GuidePlatform.Application.Dtos.ResponseDtos.UserFavorites;

namespace GuidePlatform.Application.Features.Queries.UserFavorites.GetUserFavoritesById
{
    public class GetUserFavoritesByIdQueryResponse
    {
        public int TotalCount { get; set; }
        public UserFavoritesDTO userFavorites { get; set; } = new();
    }
}
