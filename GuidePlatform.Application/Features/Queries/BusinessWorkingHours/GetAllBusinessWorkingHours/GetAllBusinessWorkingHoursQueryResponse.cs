using GuidePlatform.Application.Dtos.ResponseDtos.BusinessWorkingHours;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetAllBusinessWorkingHours
{
  public class GetAllBusinessWorkingHoursQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<BusinessWorkingHoursDTO> businessWorkingHours { get; set; } = new();
  }
}
