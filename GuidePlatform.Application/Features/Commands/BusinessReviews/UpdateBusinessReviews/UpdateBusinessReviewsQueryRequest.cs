using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.BusinessReviewsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessReviews.UpdateBusinessReviews
{
  public class UpdateBusinessReviewsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateBusinessReviewsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // İş yeri ID'si - opsiyonel güncelleme
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string? BusinessId { get; set; }

    // Yorum yapan kullanıcı ID'si - opsiyonel güncelleme
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string? ReviewerId { get; set; }

    // Değerlendirme puanı (1-5 arası) - opsiyonel güncelleme
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int? Rating { get; set; }

    // Yorum metni - opsiyonel güncelleme
    [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
    public string? Comment { get; set; }

    // Yorum doğrulanmış mı? - opsiyonel güncelleme
    public bool? IsVerified { get; set; }

    // Yorum onaylanmış mı? - opsiyonel güncelleme
    public bool? IsApproved { get; set; }

    // İkon adı - opsiyonel güncelleme
    [StringLength(100, ErrorMessage = "Icon name cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdateBusinessReviewsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // İş yeri ID'si güncelleme
      if (!string.IsNullOrWhiteSpace(request.BusinessId))
        entity.BusinessId = Guid.Parse(request.BusinessId);

      // Yorum yapan kullanıcı ID'si güncelleme
      if (!string.IsNullOrWhiteSpace(request.ReviewerId))
        entity.ReviewerId = Guid.Parse(request.ReviewerId);

      // Değerlendirme puanı güncelleme
      if (request.Rating.HasValue)
        entity.Rating = request.Rating.Value;

      // Yorum metni güncelleme
      if (request.Comment != null)
        entity.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();

      // Yorum doğrulanmış mı güncelleme
      if (request.IsVerified.HasValue)
        entity.IsVerified = request.IsVerified.Value;

      // Yorum onaylanmış mı güncelleme
      if (request.IsApproved.HasValue)
        entity.IsApproved = request.IsApproved.Value;

      // İkon adı güncelleme
      if (request.Icon != null)
        entity.Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim();

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
