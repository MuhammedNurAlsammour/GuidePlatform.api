using GuidePlatform.Application.Dtos.ResponseDtos.categories;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.categories.GetAllCategories
{
  public class GetAllCategoriesQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<categoriesDTO> categories { get; set; } = new();
  }
}
