using GuidePlatform.Application.Dtos.ResponseDtos.BusinessImages;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetAllBusinessImages
{
  public class GetAllBusinessImagesQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<BusinessImagesDTO> businessImages { get; set; } = new();
  }
}
