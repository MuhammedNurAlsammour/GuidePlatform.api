using GuidePlatform.Application.Dtos.ResponseDtos.Banners;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Banners.GetAllBanners
{
  public class GetAllBannersQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<BannersDTO> banners { get; set; } = new();
  }
}
