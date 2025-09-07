using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Subscriptions.DeleteSubscriptions
{
  public class DeleteSubscriptionsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteSubscriptionsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

