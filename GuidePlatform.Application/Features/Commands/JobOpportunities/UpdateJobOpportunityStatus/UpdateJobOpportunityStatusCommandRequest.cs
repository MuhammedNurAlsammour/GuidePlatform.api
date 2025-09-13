using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.UpdateJobOpportunityStatus
{
  /// <summary>
  /// JobOpportunity durumunu güncellemek için özel command
  /// Special command for updating JobOpportunity status
  /// </summary>
  public class UpdateJobOpportunityStatusCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateJobOpportunityStatusCommandResponse>>
  {
    [Required(ErrorMessage = "Id gerekli")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Geçersiz GUID formatı")]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status gerekli")]
    public JobOpportunityStatus Status { get; set; }

    public string? Reason { get; set; } // Red/Deny sebebi için
  }
}
