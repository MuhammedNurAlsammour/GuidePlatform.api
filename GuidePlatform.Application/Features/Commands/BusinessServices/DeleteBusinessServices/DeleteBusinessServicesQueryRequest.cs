using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessServices.DeleteBusinessServices
{
  public class DeleteBusinessServicesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteBusinessServicesCommandResponse>>
  {
    public string Id { get; set; }
  }
}

