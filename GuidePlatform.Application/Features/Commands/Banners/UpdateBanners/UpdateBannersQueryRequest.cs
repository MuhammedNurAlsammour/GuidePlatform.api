using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.BannersViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Banners.UpdateBanners
{
  public class UpdateBannersCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateBannersCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;
    public Guid? ProvinceId { get; set; }

    // Banner ile ilgili güncellenebilir alanlar - Updatable banner fields
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public byte[]? Photo { get; set; } // Eski sistem için korunuyor

    public byte[]? Thumbnail { get; set; } // Eski sistem için korunuyor

    // Yeni sistem: URL alanları - New system: URL fields
    public string? PhotoUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    [StringLength(50, ErrorMessage = "Photo content type cannot exceed 50 characters")]
    public string? PhotoContentType { get; set; }

    [StringLength(500, ErrorMessage = "Link URL cannot exceed 500 characters")]
    public string? LinkUrl { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public int? OrderIndex { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdateBannersCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Banner alanlarını güncelle - Update banner fields
      if (!string.IsNullOrWhiteSpace(request.Title))
        entity.Title = request.Title.Trim();

      if (!string.IsNullOrWhiteSpace(request.Description))
        entity.Description = request.Description.Trim();
      else if (request.Description == null)
        entity.Description = null;

      // Photo ve Thumbnail byte[] alanları artık kullanılmıyor - sadece URL'ler kullanılıyor
      // Photo and Thumbnail byte[] fields are no longer used - only URLs are used
      // if (request.Photo != null)
      //   entity.Photo = request.Photo; // Eski sistem için korunuyor

      // if (request.Thumbnail != null)
      //   entity.Thumbnail = request.Thumbnail; // Eski sistem için korunuyor

      // Yeni sistem: URL alanlarını güncelle - New system: Update URL fields
      if (!string.IsNullOrWhiteSpace(request.PhotoUrl))
        entity.PhotoUrl = request.PhotoUrl.Trim();
      else if (request.PhotoUrl == null)
        entity.PhotoUrl = null;

      if (!string.IsNullOrWhiteSpace(request.ThumbnailUrl))
        entity.ThumbnailUrl = request.ThumbnailUrl.Trim();
      else if (request.ThumbnailUrl == null)
        entity.ThumbnailUrl = null;

      if (request.ProvinceId.HasValue)
        entity.ProvinceId = request.ProvinceId.Value;

      if (!string.IsNullOrWhiteSpace(request.PhotoContentType))
        entity.PhotoContentType = request.PhotoContentType.Trim();
      else if (request.PhotoContentType == null)
        entity.PhotoContentType = null;

      if (!string.IsNullOrWhiteSpace(request.LinkUrl))
        entity.LinkUrl = request.LinkUrl.Trim();
      else if (request.LinkUrl == null)
        entity.LinkUrl = null;

      if (request.StartDate.HasValue)
        entity.StartDate = request.StartDate.Value;

      if (request.EndDate.HasValue)
        entity.EndDate = request.EndDate.Value;
      else if (request.EndDate == null)
        entity.EndDate = null;

      if (request.IsActive.HasValue)
        entity.IsActive = request.IsActive.Value;

      if (request.OrderIndex.HasValue)
        entity.OrderIndex = request.OrderIndex.Value;

      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
