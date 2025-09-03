using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Businesses.DeleteBusinesses
{
  public class DeleteBusinessesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteBusinessesCommandResponse>>
  {
    public string Id { get; set; }
  }
}

