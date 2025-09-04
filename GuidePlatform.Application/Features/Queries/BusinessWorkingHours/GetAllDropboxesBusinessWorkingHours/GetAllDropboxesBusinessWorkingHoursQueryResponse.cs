using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetAllDropboxesBusinessWorkingHours
{
	public class GetAllDropboxesBusinessWorkingHoursQueryResponse
	{
		public List<businessWorkingHoursDetailDto> businessWorkingHours { get; set; } = new List<businessWorkingHoursDetailDto>();
	}
	
	public class businessWorkingHoursDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
