using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetAllDropboxesBusinessServices
{
	public class GetAllDropboxesBusinessServicesQueryResponse
	{
		public List<businessServicesDetailDto> businessServices { get; set; } = new List<businessServicesDetailDto>();
	}
	
	public class businessServicesDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
