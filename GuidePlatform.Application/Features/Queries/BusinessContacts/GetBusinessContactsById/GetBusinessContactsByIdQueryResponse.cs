using E = GuidePlatform.Domain.Entities;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessContacts;

namespace GuidePlatform.Application.Features.Queries.BusinessContacts.GetBusinessContactsById
{
  public class GetBusinessContactsByIdQueryResponse
  {
    public int TotalCount { get; set; }
    public BusinessContactsDTO businessContacts { get; set; } = new();
  }
}
