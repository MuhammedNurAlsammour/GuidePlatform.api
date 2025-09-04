using GuidePlatform.Application.Dtos.ResponseDtos.BusinessContacts;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessContacts.GetAllBusinessContacts
{
  public class GetAllBusinessContactsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<BusinessContactsDTO> businessContacts { get; set; } = new();
  }
}
