using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.JobOpportunities.GetAllDropboxesJobOpportunities
{
	public class GetAllDropboxesJobOpportunitiesQueryResponse
	{
		public List<jobOpportunitiesDetailDto> jobOpportunities { get; set; } = new List<jobOpportunitiesDetailDto>();
	}
	
	public class jobOpportunitiesDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
