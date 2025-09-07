using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Notifications.DeleteNotifications
{
  public class DeleteNotificationsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteNotificationsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

