using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.SearchLogs.GetAllDropboxesSearchLogs
{
	public class GetAllDropboxesSearchLogsQueryResponse
	{
		public List<searchLogsDetailDto> searchLogs { get; set; } = new List<searchLogsDetailDto>();
	}
	
	public class searchLogsDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
