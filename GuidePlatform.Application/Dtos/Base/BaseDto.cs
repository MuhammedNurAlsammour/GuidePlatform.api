namespace GuidePlatform.Application.Dtos.Base
{
	/// <summary>
	/// Base class for all DTOs with common properties
	/// </summary>
	public abstract class BaseDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }	
		public Guid? CreateUserId { get; set; }
		public Guid? UpdateUserId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		public DateTime? RowCreatedDate { get; set; }
		public DateTime? RowUpdatedDate { get; set; }
		public bool RowIsActive { get; set; }
		public bool RowIsDeleted { get; set; }
	}
}
