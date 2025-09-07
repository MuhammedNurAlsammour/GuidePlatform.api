using GuidePlatform.Application.Dtos.ResponseDtos.Payments;
using GuidePlatform.Application.Dtos.Base;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Payments.GetAllPayments
{
  public class GetAllPaymentsQueryResponse : BaseResponseDto
  {
    public int TotalCount { get; set; }
    public List<PaymentsDTO> payments { get; set; } = new();
  }
}
