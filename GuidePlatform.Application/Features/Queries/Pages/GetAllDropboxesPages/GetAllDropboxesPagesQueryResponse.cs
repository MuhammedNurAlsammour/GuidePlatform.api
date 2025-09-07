using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Pages.GetAllDropboxesPages
{
	public class GetAllDropboxesPagesQueryResponse
	{
		public List<pagesDetailDto> pages { get; set; } = new List<pagesDetailDto>();
	}
	
	public class pagesDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
