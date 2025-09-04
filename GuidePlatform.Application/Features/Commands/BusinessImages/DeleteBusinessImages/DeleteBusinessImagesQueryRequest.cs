using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessImages.DeleteBusinessImages
{
  public class DeleteBusinessImagesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteBusinessImagesCommandResponse>>
  {
    public string Id { get; set; }
  }
}

