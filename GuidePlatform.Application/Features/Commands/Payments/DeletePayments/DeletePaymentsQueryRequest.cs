using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Payments.DeletePayments
{
  public class DeletePaymentsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeletePaymentsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

