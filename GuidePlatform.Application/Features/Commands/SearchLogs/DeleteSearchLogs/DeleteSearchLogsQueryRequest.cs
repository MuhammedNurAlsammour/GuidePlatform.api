using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.SearchLogs.DeleteSearchLogs
{
  public class DeleteSearchLogsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteSearchLogsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

