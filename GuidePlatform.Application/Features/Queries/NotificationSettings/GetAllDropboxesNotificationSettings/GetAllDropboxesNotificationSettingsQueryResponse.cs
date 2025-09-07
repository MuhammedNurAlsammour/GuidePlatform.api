using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.NotificationSettings.GetAllDropboxesNotificationSettings
{
	public class GetAllDropboxesNotificationSettingsQueryResponse
	{
		public List<notificationSettingsDetailDto> notificationSettings { get; set; } = new List<notificationSettingsDetailDto>();
	}
	
	public class notificationSettingsDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
