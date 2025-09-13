using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Announcements.GetAllDropboxesAnnouncements
{
	public class GetAllDropboxesAnnouncementsQueryResponse
	{
		public List<announcementsDetailDto> announcements { get; set; } = new List<announcementsDetailDto>();
	}
	
	public class announcementsDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
