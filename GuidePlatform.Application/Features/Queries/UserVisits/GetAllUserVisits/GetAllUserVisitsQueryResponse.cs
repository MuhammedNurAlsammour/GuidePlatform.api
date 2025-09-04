using GuidePlatform.Application.Dtos.ResponseDtos.UserVisits;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.UserVisits.GetAllUserVisits
{
  public class GetAllUserVisitsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<UserVisitsDTO> userVisits { get; set; } = new();
  }
}
