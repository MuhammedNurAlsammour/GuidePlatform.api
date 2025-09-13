using GuidePlatform.Application.Dtos.Response;

namespace GuidePlatform.Application.Features.Commands.Banners.CreateBannerWithImage
{
  /// <summary>
  /// Banner oluşturma işlemi sonucu
  /// </summary>
  public class CreateBannerWithImageCommandResponse
  {
    public Guid Id { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? ImageUrl { get; set; }
  }
}
