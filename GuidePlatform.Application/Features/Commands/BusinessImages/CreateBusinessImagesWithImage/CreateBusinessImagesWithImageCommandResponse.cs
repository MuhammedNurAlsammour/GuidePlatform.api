using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessImages.CreateBusinessImagesWithImage
{
  /// <summary>
  /// BusinessImages oluşturma işlemi sonucu
  /// </summary>
  public class CreateBusinessImagesWithImageCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
  }
}
