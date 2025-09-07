using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Banners.DeleteBanners
{
  public class DeleteBannersCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteBannersCommandResponse>>
  {
    public string Id { get; set; }
  }
}

