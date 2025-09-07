using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.NotificationSettings.DeleteNotificationSettings
{
  public class DeleteNotificationSettingsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteNotificationSettingsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

