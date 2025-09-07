using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.Articles.CreateArticles
{
  public class CreateArticlesCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
