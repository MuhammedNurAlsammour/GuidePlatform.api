using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetAllBusinessImages
{
  public class GetAllBusinessImagesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessImagesQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🏢 İşletme bilgileri - Business information
    public Guid? BusinessId { get; set; }                // İşletme ID - Business ID

    // 🖼️ Resim bilgileri - Image information
    public bool? IsPrimary { get; set; }                 // Ana resim mi - Is primary image
    public int? SortOrder { get; set; }                  // Sıralama düzeni - Sort order

  }
}
