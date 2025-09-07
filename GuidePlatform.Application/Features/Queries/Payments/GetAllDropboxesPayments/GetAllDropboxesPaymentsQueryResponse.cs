using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Payments.GetAllDropboxesPayments
{
	public class GetAllDropboxesPaymentsQueryResponse
	{
		public List<paymentsDetailDto> payments { get; set; } = new List<paymentsDetailDto>();
	}
	
	public class paymentsDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
