using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.Banners.CreateBanners
{
  public class CreateBannersCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
