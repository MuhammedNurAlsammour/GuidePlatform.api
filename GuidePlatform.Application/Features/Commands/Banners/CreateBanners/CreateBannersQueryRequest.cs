using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.BannersViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.Banners.CreateBanners
{
  public class CreateBannersCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBannersCommandResponse>>
  {
    // Banner ile ilgili temel bilgiler - Basic banner information
    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public byte[]? Photo { get; set; }

    public byte[]? Thumbnail { get; set; }

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
        Photo = request.Photo,
        Thumbnail = request.Thumbnail,
        PhotoContentType = request.PhotoContentType,
        LinkUrl = request.LinkUrl,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        IsActive = request.IsActive,
        OrderIndex = request.OrderIndex,
        Icon = request.Icon,
      };
    }
  }
}

