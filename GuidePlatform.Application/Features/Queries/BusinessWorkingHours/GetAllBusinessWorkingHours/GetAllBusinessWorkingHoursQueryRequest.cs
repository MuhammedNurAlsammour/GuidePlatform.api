using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetAllBusinessWorkingHours
{
  public class GetAllBusinessWorkingHoursQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessWorkingHoursQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🏢 İşletme bilgileri - Business information
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format for BusinessId")]
    public string? BusinessId { get; set; }               // İşletme ID - Business ID

    // ⏰ Çalışma saatleri bilgileri - Working hours information
    public int? DayOfWeek { get; set; }                   // Haftanın günü - Day of week (0-6)
    public TimeSpan? OpenTime { get; set; }               // Açılış saati - Open time
    public TimeSpan? CloseTime { get; set; }              // Kapanış saati - Close time
    public bool? IsClosed { get; set; }                   // Kapalı mı - Is closed
    public string? Icon { get; set; }                     // İkon - Icon
  }
}
