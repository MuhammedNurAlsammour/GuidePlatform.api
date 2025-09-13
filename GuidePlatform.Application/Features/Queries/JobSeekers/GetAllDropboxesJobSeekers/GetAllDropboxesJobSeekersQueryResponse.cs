using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.JobSeekers.GetAllDropboxesJobSeekers
{
	public class GetAllDropboxesJobSeekersQueryResponse
	{
		public List<jobSeekersDetailDto> jobSeekers { get; set; } = new List<jobSeekersDetailDto>();
	}
	
	public class jobSeekersDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
