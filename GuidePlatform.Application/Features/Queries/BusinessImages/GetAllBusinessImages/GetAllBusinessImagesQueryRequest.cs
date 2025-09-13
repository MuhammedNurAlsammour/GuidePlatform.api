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
    public int? ImageType { get; set; } // Resim tipi - Image type (0:profile, 1:gallery, 2:menu, 3:banner, 4:logo, 5:interior, 6:exterior, 7:food, 8:kitchen, 9:atmosphere, 10:design, 11:dessert,12:imageJobSeekerSponsored,13:imageJobOpportunitieSponsored)

    // 🖼️ Resim bilgileri - Image information
    public bool? IsPrimary { get; set; }                 // Ana resim mi - Is primary image
    public int? SortOrder { get; set; }                  // Sıralama düzeni - Sort order

  }
}
