using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.BulkApproveJobOpportunities
{
  /// <summary>
  /// Birden fazla JobOpportunity'yi toplu olarak onaylamak için command
  /// Command for bulk approving multiple JobOpportunities
  /// </summary>
  public class BulkApproveJobOpportunitiesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<BulkApproveJobOpportunitiesCommandResponse>>
  {
    [Required(ErrorMessage = "JobOpportunity ID'leri gerekli")]
    [MinLength(1, ErrorMessage = "En az bir JobOpportunity ID'si gerekli")]
    public List<string> JobOpportunityIds { get; set; } = new List<string>();

    [Required(ErrorMessage = "Status gerekli")]
    public JobOpportunityStatus Status { get; set; }

    public string? Reason { get; set; } // Toplu işlem sebebi için
  }
}
