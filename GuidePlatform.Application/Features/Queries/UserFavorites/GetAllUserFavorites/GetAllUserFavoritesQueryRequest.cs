using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.UserFavorites.GetAllUserFavorites
{
  public class GetAllUserFavoritesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllUserFavoritesQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🏢 İşletme bilgileri - Business information
    public Guid? BusinessId { get; set; }                // İşletme ID - Business ID

    // ⭐ Favori bilgileri - Favorite information
    public string? Icon { get; set; }                    // İkon - Icon
  }
}
