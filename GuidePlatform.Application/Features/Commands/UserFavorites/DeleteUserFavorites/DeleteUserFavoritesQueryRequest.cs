using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.UserFavorites.DeleteUserFavorites
{
  public class DeleteUserFavoritesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteUserFavoritesCommandResponse>>
  {
    public string Id { get; set; }
  }
}

