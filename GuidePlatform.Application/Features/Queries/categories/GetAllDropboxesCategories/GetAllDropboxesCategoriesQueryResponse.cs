using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.categories.GetAllDropboxesCategories
{
	public class GetAllDropboxesCategoriesQueryResponse
	{
		public List<categoriesDetailDto> categories { get; set; } = new List<categoriesDetailDto>();
	}
	
	public class categoriesDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
