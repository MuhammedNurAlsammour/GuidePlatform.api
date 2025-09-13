using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Files.GetAllDropboxesFiles
{
	public class GetAllDropboxesFilesQueryResponse
	{
		public List<filesDetailDto> files { get; set; } = new List<filesDetailDto>();
	}
	
	public class filesDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
