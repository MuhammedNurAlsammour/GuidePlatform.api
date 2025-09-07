using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Banners.GetAllDropboxesBanners
{
	public class GetAllDropboxesBannersQueryResponse
	{
		public List<bannersDetailDto> banners { get; set; } = new List<bannersDetailDto>();
	}
	
	public class bannersDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
