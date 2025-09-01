namespace GuidePlatform.Application.Dtos
{
	/// <summary>
	/// Auth kullanıcı detay bilgilerini içeren DTO
	/// Username ve CustomerName bilgileri ile birlikte
	/// </summary>
	public class AuthUserDetailDto
	{
		public Guid AuthUserId { get; set; }
		public string? AuthUserName { get; set; }      // AspNetUsers.UserName
		public string? AuthCustomerName { get; set; }  // Customers.Name
		public Guid CustomerId { get; set; }          // AspNetUsers.CustomerId
	}
}