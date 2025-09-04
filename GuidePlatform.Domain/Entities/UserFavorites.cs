using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class UserFavoritesViewModel : BaseEntity
  {
    // İşletme ID'si - hangi işletmeyi favorilere ekledi
    public Guid BusinessId { get; set; }

    // İkon türü (varsayılan: favorite)
    public string Icon { get; set; } = "favorite";
  }
}