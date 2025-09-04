using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.UserVisits.DeleteUserVisits
{
  public class DeleteUserVisitsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteUserVisitsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

