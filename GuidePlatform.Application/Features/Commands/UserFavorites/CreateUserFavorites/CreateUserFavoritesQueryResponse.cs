using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.UserFavorites.CreateUserFavorites
{
  public class CreateUserFavoritesCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
