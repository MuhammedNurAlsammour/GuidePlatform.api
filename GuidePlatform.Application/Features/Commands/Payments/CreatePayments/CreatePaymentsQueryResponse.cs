using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Commands.Payments.CreatePayments
{
  public class CreatePaymentsCommandResponse : BaseResponseDto
  {
    public Guid Id { get; set; }
  }
}
