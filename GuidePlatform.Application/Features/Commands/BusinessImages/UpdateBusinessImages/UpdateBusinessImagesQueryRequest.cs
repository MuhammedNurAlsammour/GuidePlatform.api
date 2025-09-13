using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.BusinessImagesViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessImages.UpdateBusinessImages
{
  public class UpdateBusinessImagesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateBusinessImagesCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // İşletme ID'si - business_id
    [Required(ErrorMessage = "BusinessId is required")]
    public Guid BusinessId { get; set; }

    // Fotoğraf verisi - photo (eski sistem için)
    public byte[]? Photo { get; set; }

    // Küçük resim verisi - thumbnail (eski sistem için)
    public byte[]? Thumbnail { get; set; }

    // Fotoğraf URL'si - photo_url (yeni sistem için)
    public string? PhotoUrl { get; set; }

    // Küçük resim URL'si - thumbnail_url (yeni sistem için)
    public string? ThumbnailUrl { get; set; }

    // Fotoğraf içerik tipi - photo_content_type
    [StringLength(50, ErrorMessage = "PhotoContentType cannot exceed 50 characters")]
    public string? PhotoContentType { get; set; }

    // Alternatif metin - alt_text
    [StringLength(255, ErrorMessage = "AltText cannot exceed 255 characters")]
    public string? AltText { get; set; }

    // Ana fotoğraf mı - is_primary
    public bool IsPrimary { get; set; }

    // Sıralama düzeni - sort_order
    public int SortOrder { get; set; }

    // İkon - icon
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    // Resim tipi - image_type
    [Range(0, 11, ErrorMessage = "ImageType must be between 0 and 11")]
    public int ImageType { get; set; }

    public static O Map(O entity, UpdateBusinessImagesCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      // Güncelleme bilgileri
      entity.BusinessId = request.BusinessId;
      entity.Photo = request.Photo; // Eski sistem için korunuyor
      entity.Thumbnail = request.Thumbnail; // Eski sistem için korunuyor
      entity.PhotoUrl = request.PhotoUrl; // Yeni sistem: URL'yi güncelle
      entity.ThumbnailUrl = request.ThumbnailUrl; // Yeni sistem: URL'yi güncelle
      entity.PhotoContentType = request.PhotoContentType;
      entity.AltText = request.AltText;
      entity.IsPrimary = request.IsPrimary;
      entity.SortOrder = request.SortOrder;
      entity.Icon = request.Icon;
      entity.ImageType = request.ImageType;

      // Auth bilgileri
      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      if (updateUserId.HasValue)
        entity.UpdateUserId = updateUserId.Value;

      return entity;
    }
  }
}
