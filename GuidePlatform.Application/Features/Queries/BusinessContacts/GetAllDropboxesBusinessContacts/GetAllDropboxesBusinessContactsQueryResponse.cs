using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessContacts.GetAllDropboxesBusinessContacts
{
	public class GetAllDropboxesBusinessContactsQueryResponse
	{
		public List<businessContactsDetailDto> businessContacts { get; set; } = new List<businessContactsDetailDto>();
	}
	
	public class businessContactsDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
