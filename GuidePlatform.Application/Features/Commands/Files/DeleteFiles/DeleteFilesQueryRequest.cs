using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Files.DeleteFiles
{
  public class DeleteFilesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteFilesCommandResponse>>
  {
    public string Id { get; set; }
  }
}

