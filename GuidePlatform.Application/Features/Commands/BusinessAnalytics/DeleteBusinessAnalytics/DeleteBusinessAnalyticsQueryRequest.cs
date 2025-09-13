using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessAnalytics.DeleteBusinessAnalytics
{
  public class DeleteBusinessAnalyticsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteBusinessAnalyticsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

