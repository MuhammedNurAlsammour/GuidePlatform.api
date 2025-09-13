

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.BusinessAnalytics
{
  public class BusinessAnalyticsDTO : BaseResponseDTO
  {
    // İş analitik verileri için gerekli alanlar
    public Guid BusinessId { get; set; }
    public DateTime Date { get; set; }
    public int ViewsCount { get; set; }
    public int ContactsCount { get; set; }
    public int ReviewsCount { get; set; }
    public int FavoritesCount { get; set; }
    public string Icon { get; set; } = "analytics";
  }
}
