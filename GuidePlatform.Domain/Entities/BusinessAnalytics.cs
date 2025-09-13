using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class BusinessAnalyticsViewModel : BaseEntity
  {
    // İş analitik verileri için gerekli alanlar
    public Guid BusinessId { get; set; }
    public DateTime Date { get; set; }
    public int ViewsCount { get; set; } = 0;
    public int ContactsCount { get; set; } = 0;
    public int ReviewsCount { get; set; } = 0;
    public int FavoritesCount { get; set; } = 0;
    public string Icon { get; set; } = "analytics";
  }
}