using GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetAllBusinessReviews
{
  public class GetAllBusinessReviewsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<BusinessReviewsDTO> businessReviews { get; set; } = new();
  }
}
