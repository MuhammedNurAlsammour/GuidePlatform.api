using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.BannersViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Application.Services;


namespace GuidePlatform.Application.Features.Commands.Banners.CreateBanners
{
  public class CreateBannersCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBannersCommandResponse>>
  {
    // Banner ile ilgili temel bilgiler - Basic banner information
    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string Title { get; set; } = string.Empty;

    public Guid? ProvinceId { get; set; }

    public string? Description { get; set; }

    public byte[]? Photo { get; set; } // Eski sistem için korunuyor

    public byte[]? Thumbnail { get; set; } // Eski sistem için korunuyor

    // Yeni sistem: URL alanları - New system: URL fields
    public string? PhotoUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    // Eski sistem için alanlar - Old system fields
    public string? PhotoPath { get; set; }
    public string? PhotoBase64 { get; set; }

    [StringLength(50, ErrorMessage = "Photo content type cannot exceed 50 characters")]
    public string? PhotoContentType { get; set; }

    [StringLength(500, ErrorMessage = "Link URL cannot exceed 500 characters")]
    public string? LinkUrl { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public int OrderIndex { get; set; } = 0;

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "image";

    public static O Map(CreateBannersCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId
        Title = request.Title,
        Description = request.Description,
        Photo = null, // Eski sistem için korunuyor ama boş bırakılıyor
        Thumbnail = null, // Eski sistem için korunuyor ama boş bırakılıyor
        // PhotoUrl ve ThumbnailUrl sadece gerçek URL'ler ise kaydet (file path değilse)
        PhotoUrl = IsValidUrl(request.PhotoUrl) ? request.PhotoUrl : null,
        ThumbnailUrl = IsValidUrl(request.ThumbnailUrl) ? request.ThumbnailUrl : null,
        PhotoContentType = request.PhotoContentType,
        LinkUrl = request.LinkUrl,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        IsActive = request.IsActive,
        OrderIndex = request.OrderIndex,
        Icon = request.Icon,
      };
    }

    public BannerImageUploadDto ToPhotoUploadDto(Guid bannerId)
    {
      // إذا كان PhotoUrl هو URL صحيح، استخدمه كـ PhotoPath
      string? photoPathToUse = PhotoPath;
      if (string.IsNullOrEmpty(photoPathToUse) && !string.IsNullOrEmpty(PhotoUrl))
      {
        if (PhotoUrl.StartsWith("http://") || PhotoUrl.StartsWith("https://"))
        {
          photoPathToUse = PhotoUrl; // استخدم URL كـ PhotoPath
        }
      }

      return new BannerImageUploadDto
      {
        BannerId = bannerId,
        PhotoPath = photoPathToUse,
        PhotoBase64 = PhotoBase64
      };
    }

    private static bool IsValidUrl(string? url)
    {
      if (string.IsNullOrEmpty(url))
        return false;

      // File path kontrolü - File path check
      if (url.StartsWith("C:\\") || url.StartsWith("D:\\") || url.StartsWith("/") || url.StartsWith("\\"))
        return false;

      // Gerçek URL kontrolü - Real URL check
      return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
             (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
  }
}

