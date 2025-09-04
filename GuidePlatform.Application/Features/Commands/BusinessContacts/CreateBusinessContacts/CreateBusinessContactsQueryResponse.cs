using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessContacts.CreateBusinessContacts
{
  public class CreateBusinessContactsCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
