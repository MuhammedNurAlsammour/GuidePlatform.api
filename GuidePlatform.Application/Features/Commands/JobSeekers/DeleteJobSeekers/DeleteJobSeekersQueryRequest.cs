using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.DeleteJobSeekers
{
  public class DeleteJobSeekersCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteJobSeekersCommandResponse>>
  {
    public string Id { get; set; }
  }
}

