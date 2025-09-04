using GuidePlatform.Application.Dtos.ResponseDtos.UserFavorites;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.UserFavorites.GetAllUserFavorites
{
  public class GetAllUserFavoritesQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<UserFavoritesDTO> userFavorites { get; set; } = new();
  }
}
