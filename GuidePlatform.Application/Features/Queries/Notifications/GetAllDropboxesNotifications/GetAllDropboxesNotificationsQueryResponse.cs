using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Notifications.GetAllDropboxesNotifications
{
	public class GetAllDropboxesNotificationsQueryResponse
	{
		public List<notificationsDetailDto> notifications { get; set; } = new List<notificationsDetailDto>();
	}
	
	public class notificationsDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
