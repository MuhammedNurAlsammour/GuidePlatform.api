namespace GuidePlatform.Application.Dtos.ResponseDtos
{
	public class BaseResponseDTO
	{
		public Guid Id { get; set; }
		public Guid? CustomerId { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public string? AuthCustomerName { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public Guid? CreateUserId { get; set; }
		public string? CreateUserName { get; set; }
		public Guid? UpdateUserId { get; set; }
		public string? UpdateUserName { get; set; }
		public DateTime RowCreatedDate { get; set; }
		public DateTime RowUpdatedDate { get; set; }
		public bool RowIsActive { get; set; }
		public bool RowIsDeleted { get; set; }
	}
}