using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Articles.DeleteArticles
{
  public class DeleteArticlesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteArticlesCommandResponse>>
  {
    public string Id { get; set; }
  }
}

