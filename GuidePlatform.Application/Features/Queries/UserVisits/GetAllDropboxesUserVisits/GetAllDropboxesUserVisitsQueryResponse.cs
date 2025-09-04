using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.UserVisits.GetAllDropboxesUserVisits
{
	public class GetAllDropboxesUserVisitsQueryResponse
	{
		public List<userVisitsDetailDto> userVisits { get; set; } = new List<userVisitsDetailDto>();
	}
	
	public class userVisitsDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
