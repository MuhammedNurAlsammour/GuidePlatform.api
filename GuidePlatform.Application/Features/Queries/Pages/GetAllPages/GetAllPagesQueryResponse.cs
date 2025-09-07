using GuidePlatform.Application.Dtos.ResponseDtos.Pages;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Pages.GetAllPages
{
  public class GetAllPagesQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<PagesDTO> pages { get; set; } = new();
  }
}
