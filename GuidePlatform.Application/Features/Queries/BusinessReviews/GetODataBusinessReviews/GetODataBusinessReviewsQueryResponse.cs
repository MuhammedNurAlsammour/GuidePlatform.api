using GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews;
using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetODataBusinessReviews
{
  public class GetODataBusinessReviewsQueryResponse : BaseResponseDto
  {
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int TotalCount { get; set; }
    public List<BusinessReviewsDTO> BusinessReviews { get; set; } = new();
    public int? Count { get; set; }
    public object? Pagination { get; set; }
  }
}
