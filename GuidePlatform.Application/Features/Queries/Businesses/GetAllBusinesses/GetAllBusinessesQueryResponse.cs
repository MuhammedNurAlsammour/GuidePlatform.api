using GuidePlatform.Application.Dtos.ResponseDtos.Businesses;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Businesses.GetAllBusinesses
{
  public class GetAllBusinessesQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<BusinessesDTO> businesses { get; set; } = new();
  }
}
