

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.BusinessWorkingHours
{
  public class BusinessWorkingHoursDTO : BaseResponseDTO
  {
    // İşletme ID'si - hangi işletmeye ait çalışma saatleri
    public Guid BusinessId { get; set; }

    // Haftanın günü (1=Pazartesi, 2=Salı, ..., 7=Pazar)
    public int DayOfWeek { get; set; }

    // Açılış saati
    public TimeSpan? OpenTime { get; set; }

    // Kapanış saati
    public TimeSpan? CloseTime { get; set; }

    // O gün kapalı mı?
    public bool IsClosed { get; set; } = false;

    // İkon türü (varsayılan: schedule)
    public string Icon { get; set; } = "schedule";
  }
}
