using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.BusinessReviewsViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.BusinessReviews.CreateBusinessReviews
{
  public class CreateBusinessReviewsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBusinessReviewsCommandResponse>>
  {
    // İş yeri ID'si - zorunlu alan
    [Required(ErrorMessage = "Business ID is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string BusinessId { get; set; } = string.Empty;

    // Yorum yapan kullanıcı ID'si - zorunlu alan
    [Required(ErrorMessage = "Reviewer ID is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string ReviewerId { get; set; } = string.Empty;

    // Değerlendirme puanı (1-5 arası) - zorunlu alan
    [Required(ErrorMessage = "Rating is required")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    // Yorum metni - opsiyonel
    [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
    public string? Comment { get; set; }

    // Yorum doğrulanmış mı? - opsiyonel, varsayılan false
    public bool IsVerified { get; set; } = false;

    // Yorum onaylanmış mı? - opsiyonel, varsayılan true
    public bool IsApproved { get; set; } = true;

    // İkon adı - opsiyonel, varsayılan 'rate_review'
    [StringLength(100, ErrorMessage = "Icon name cannot exceed 100 characters")]
    public string? Icon { get; set; } = "rate_review";

    public static O Map(CreateBusinessReviewsCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId,
        BusinessId = Guid.Parse(request.BusinessId),
        ReviewerId = Guid.Parse(request.ReviewerId),
        Rating = request.Rating,
        Comment = request.Comment,
        IsVerified = request.IsVerified,
        IsApproved = request.IsApproved,
        Icon = request.Icon,
      };
    }
  }
}

