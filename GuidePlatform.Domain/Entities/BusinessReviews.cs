using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class BusinessReviewsViewModel : BaseEntity
  {
    // İş yeri ID'si - hangi işletme için yorum yapıldığı
    public Guid BusinessId { get; set; }

    // Yorum yapan kullanıcı ID'si
    public Guid ReviewerId { get; set; }

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