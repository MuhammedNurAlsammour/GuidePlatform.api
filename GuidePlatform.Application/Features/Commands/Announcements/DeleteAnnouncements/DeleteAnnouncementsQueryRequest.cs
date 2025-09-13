using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Announcements.DeleteAnnouncements
{
  public class DeleteAnnouncementsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteAnnouncementsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

