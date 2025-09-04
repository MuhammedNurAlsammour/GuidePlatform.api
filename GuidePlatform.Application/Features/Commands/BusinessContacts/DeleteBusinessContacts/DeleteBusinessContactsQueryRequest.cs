using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessContacts.DeleteBusinessContacts
{
  public class DeleteBusinessContactsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteBusinessContactsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

