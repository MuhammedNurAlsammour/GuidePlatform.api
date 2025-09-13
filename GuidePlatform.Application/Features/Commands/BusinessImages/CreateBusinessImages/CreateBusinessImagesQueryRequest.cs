using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.BusinessImagesViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Application.Services;


namespace GuidePlatform.Application.Features.Commands.BusinessImages.CreateBusinessImages
{
  public class CreateBusinessImagesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBusinessImagesCommandResponse>>
  {

    // İşletme ID'si - business_id
    [Required(ErrorMessage = "BusinessId is required")]
    public Guid BusinessId { get; set; }

    // Fotoğraf yolu - photo_path (eski sistem için)
    public string? PhotoPath { get; set; }

    // Fotoğraf Base64 - photo_base64 (eski sistem için)
    public string? PhotoBase64 { get; set; }

    // Fotoğraf URL'si - photo_url (yeni sistem için)
    public string? PhotoUrl { get; set; }

    // Küçük resim URL'si - thumbnail_url (yeni sistem için)
    public string? ThumbnailUrl { get; set; }

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


    public static O Map(CreateBusinessImagesCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        BusinessId = request.BusinessId,
        Photo = null, // Eski sistem için korunuyor
        Thumbnail = null, // Eski sistem için korunuyor
        PhotoUrl = request.PhotoUrl, // Yeni sistem: URL'yi kaydet
        ThumbnailUrl = request.ThumbnailUrl, // Yeni sistem: URL'yi kaydet
        PhotoContentType = "image/jpeg", // Service tarafından ayarlanacak
        AltText = request.AltText,
        IsPrimary = request.IsPrimary,
        SortOrder = request.SortOrder,
        Icon = request.Icon,
        ImageType = request.ImageType,
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId,
        RowIsActive = true,
        RowIsDeleted = false
      };
    }

    public BusinessImageUploadDto ToPhotoUploadDto(Guid businessId, Guid businessImageId)
    {
      return new BusinessImageUploadDto
      {
        BusinessId = businessId,
        BusinessImageId = businessImageId,
        PhotoPath = PhotoPath,
        PhotoBase64 = PhotoBase64
      };
    }
  }
}

