namespace GuidePlatform.Application.Dtos
{
	public class CategoryDetailDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public Guid? AuthUserId { get; set; }
		public Guid? CustomerId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		public bool RowIsActive { get; set; }
		public bool RowIsDeleted { get; set; }
		public byte[]? Photo { get; set; }
		public byte[]? Thumbnail { get; set; }
	}
}