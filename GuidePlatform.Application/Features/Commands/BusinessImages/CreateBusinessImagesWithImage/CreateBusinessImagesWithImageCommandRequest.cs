using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GuidePlatform.Application.Features.Commands.BusinessImages.CreateBusinessImagesWithImage
{
  /// <summary>
  /// Gelen resim dosyasıyla yeni BusinessImages oluşturma isteği
  /// </summary>
  public class CreateBusinessImagesWithImageCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBusinessImagesWithImageCommandResponse>>
  {
    // İşletme ID'si - business_id
    [Required(ErrorMessage = "BusinessId is required")]
    public Guid BusinessId { get; set; }

    // Gelen dosya
    public IFormFile? PhotoFile { get; set; }

    // Alternatif metin - alt_text
    [StringLength(255, ErrorMessage = "AltText cannot exceed 255 characters")]
    public string? AltText { get; set; }

    // Ana fotoğraf mı - is_primary
    public bool IsPrimary { get; set; } = false;

    // Sıralama düzeni - sort_order
    public int SortOrder { get; set; } = 0;

    // İkon - icon
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; } = "image";

    // Resim tipi - image_type
    [Range(0, 11, ErrorMessage = "ImageType must be between 0 and 11")]
    public int ImageType { get; set; } = 1;
  }
}
