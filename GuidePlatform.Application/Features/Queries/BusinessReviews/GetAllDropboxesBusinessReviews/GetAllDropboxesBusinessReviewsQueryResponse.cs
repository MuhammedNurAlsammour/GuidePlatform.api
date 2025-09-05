using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetAllDropboxesBusinessReviews
{
	public class GetAllDropboxesBusinessReviewsQueryResponse
	{
		public List<businessReviewsDetailDto> businessReviews { get; set; } = new List<businessReviewsDetailDto>();
	}
	
	public class businessReviewsDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
