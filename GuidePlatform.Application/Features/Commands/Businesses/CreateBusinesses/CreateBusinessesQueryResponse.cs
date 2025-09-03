using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.Businesses.CreateBusinesses
{
  public class CreateBusinessesCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
