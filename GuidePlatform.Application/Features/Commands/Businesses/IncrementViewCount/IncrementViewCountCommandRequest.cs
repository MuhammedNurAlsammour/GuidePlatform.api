using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Commands.Businesses.IncrementViewCount
{
  public class IncrementViewCountCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<IncrementViewCountCommandResponse>>
  {
    [Required(ErrorMessage = "Business ID zorunludur.")]
    public Guid BusinessId { get; set; }

    // İsteğe bağlı: Ziyaretçi bilgileri (analytics için)
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RefererUrl { get; set; }
  }
}
