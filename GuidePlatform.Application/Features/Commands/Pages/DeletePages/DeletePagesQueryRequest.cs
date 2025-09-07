using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Pages.DeletePages
{
  public class DeletePagesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeletePagesCommandResponse>>
  {
    public string Id { get; set; }
  }
}

