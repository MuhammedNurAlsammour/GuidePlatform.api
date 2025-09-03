using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Businesses.GetAllDropboxesBusinesses
{
	public class GetAllDropboxesBusinessesQueryResponse
	{
		public List<businessesDetailDto> Businesses { get; set; } = new List<businessesDetailDto>();
	}
	
	public class businessesDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
