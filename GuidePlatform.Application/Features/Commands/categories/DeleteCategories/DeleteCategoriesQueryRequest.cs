using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.categories.DeleteCategories
{
  public class DeleteCategoriesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<DeleteCategoriesCommandResponse>>
  {
    public string Id { get; set; }
  }
}

