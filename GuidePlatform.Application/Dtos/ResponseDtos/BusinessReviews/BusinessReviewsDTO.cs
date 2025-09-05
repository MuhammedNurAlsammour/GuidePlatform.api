
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews
{
  public class BusinessReviewsDTO : BaseResponseDTO
  {
    // İş yeri ID'si
    public Guid BusinessId { get; set; }

    // İş yeri adı (join ile gelecek)
    public string? BusinessName { get; set; }

    // Yorum yapan kullanıcı ID'si
    public Guid ReviewerId { get; set; }

    // Yorum yapan kullanıcı adı (join ile gelecek)
    public string? ReviewerName { get; set; }

    // Değerlendirme puanı (1-5 arası)
    public int Rating { get; set; }

    // Yorum metni
    public string? Comment { get; set; }

    // Yorum doğrulanmış mı?
    public bool IsVerified { get; set; }

    // Yorum onaylanmış mı?
    public bool IsApproved { get; set; }

    // İkon adı
    public string? Icon { get; set; }
  }
}
