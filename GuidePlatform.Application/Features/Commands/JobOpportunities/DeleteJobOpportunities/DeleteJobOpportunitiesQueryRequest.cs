using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.DeleteJobOpportunities
{
  public class DeleteJobOpportunitiesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteJobOpportunitiesCommandResponse>>
  {
    public string Id { get; set; }
  }
}

