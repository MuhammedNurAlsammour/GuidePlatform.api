

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.UserVisits
{
  public class UserVisitsDTO : BaseResponseDTO
  {
    // İşletme ID'si - hangi işletmeyi ziyaret etti
    public Guid BusinessId { get; set; }

    // Ziyaret tarihi
    public DateTime? VisitDate { get; set; }

    // Ziyaret türü (view, click, visit, vb.)
    public string VisitType { get; set; } = "view";

    // İkon türü (varsayılan: visibility)
    public string Icon { get; set; } = "visibility";
  }
}
