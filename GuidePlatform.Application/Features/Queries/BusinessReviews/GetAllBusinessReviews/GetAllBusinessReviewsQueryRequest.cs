using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetAllBusinessReviews
{
  public class GetAllBusinessReviewsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessReviewsQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // İş yeri ID'si ile filtreleme - opsiyonel
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string? BusinessId { get; set; }

    // Yorum yapan kullanıcı ID'si ile filtreleme - opsiyonel
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string? ReviewerId { get; set; }

    // Sadece onaylanmış yorumları getir - opsiyonel
    public bool? IsApproved { get; set; }

    // Sadece doğrulanmış yorumları getir - opsiyonel
    public bool? IsVerified { get; set; }

    // Minimum rating ile filtreleme - opsiyonel
    [Range(1, 5, ErrorMessage = "MinRating must be between 1 and 5")]
    public int? MinRating { get; set; }

    // Maximum rating ile filtreleme - opsiyonel
    [Range(1, 5, ErrorMessage = "MaxRating must be between 1 and 5")]
    public int? MaxRating { get; set; }
  }
}
