using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessWorkingHours.DeleteBusinessWorkingHours
{
  public class DeleteBusinessWorkingHoursCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteBusinessWorkingHoursCommandResponse>>
  {
    public string Id { get; set; }
  }
}

