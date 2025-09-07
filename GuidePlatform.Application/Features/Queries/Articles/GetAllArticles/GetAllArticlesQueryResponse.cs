using GuidePlatform.Application.Dtos.ResponseDtos.Articles;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Articles.GetAllArticles
{
  public class GetAllArticlesQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<ArticlesDTO> articles { get; set; } = new();
  }
}
