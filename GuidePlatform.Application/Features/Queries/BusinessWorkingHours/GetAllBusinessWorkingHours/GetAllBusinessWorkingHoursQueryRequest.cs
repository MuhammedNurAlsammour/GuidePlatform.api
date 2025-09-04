using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetAllBusinessWorkingHours
{
  public class GetAllBusinessWorkingHoursQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessWorkingHoursQueryResponse>>
  {
    // İşletme ID'si - hangi işletmenin çalışma saatlerini getirmek için (opsiyonel)
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format for BusinessId")]
    public string? BusinessId { get; set; }
  }
}
