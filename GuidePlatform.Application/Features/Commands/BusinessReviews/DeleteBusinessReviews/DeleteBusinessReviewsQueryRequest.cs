using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessReviews.DeleteBusinessReviews
{
  public class DeleteBusinessReviewsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteBusinessReviewsCommandResponse>>
  {
    public string Id { get; set; }
  }
}

