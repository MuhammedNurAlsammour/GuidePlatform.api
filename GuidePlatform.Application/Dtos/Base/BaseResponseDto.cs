namespace GuidePlatform.Application.Dtos.Base
{
	/// <summary>
	/// Base class for all Response DTOs with common properties
	/// </summary>
	public abstract class BaseResponseDto
	{
		public int StatusCode { get; set; }
		public string? Message { get; set; }
		public DateTime? Timestamp { get; set; } = DateTime.UtcNow;
	}
}
