using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetAllDropboxesBusinessAnalytics
{
	public class GetAllDropboxesBusinessAnalyticsQueryResponse
	{
		public List<businessAnalyticsDetailDto> businessAnalytics { get; set; } = new List<businessAnalyticsDetailDto>();
	}
	
	public class businessAnalyticsDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
