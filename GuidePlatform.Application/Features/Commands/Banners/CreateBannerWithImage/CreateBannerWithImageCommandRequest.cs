using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GuidePlatform.Application.Features.Commands.Banners.CreateBannerWithImage
{
  /// <summary>
  /// Gelen resim dosyasıyla yeni banner oluşturma isteği
  /// </summary>
  public class CreateBannerWithImageCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBannerWithImageCommandResponse>>
  {
    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string Title { get; set; } = string.Empty;

    public Guid? ProvinceId { get; set; }

    public string? Description { get; set; }
    public string? PhotoContentType { get; set; }

    // Gelen dosya
    public IFormFile? PhotoFile { get; set; }

    [StringLength(500, ErrorMessage = "Link URL cannot exceed 500 characters")]
    public string? LinkUrl { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public int OrderIndex { get; set; } = 0;

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "image";
  }
}
