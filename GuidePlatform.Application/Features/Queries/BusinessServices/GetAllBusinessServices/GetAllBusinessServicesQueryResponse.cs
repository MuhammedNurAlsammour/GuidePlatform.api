using GuidePlatform.Application.Dtos.ResponseDtos.BusinessServices;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetAllBusinessServices
{
  public class GetAllBusinessServicesQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<BusinessServicesDTO> businessServices { get; set; } = new();
  }
}
