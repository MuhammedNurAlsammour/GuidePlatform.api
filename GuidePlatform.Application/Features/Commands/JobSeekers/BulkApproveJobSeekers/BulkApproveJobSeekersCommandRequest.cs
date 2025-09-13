using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.BulkApproveJobSeekers
{
  /// <summary>
  /// Birden fazla JobSeeker'ı toplu olarak onaylamak için command
  /// Command for bulk approving multiple JobSeekers
  /// </summary>
  public class BulkApproveJobSeekersCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<BulkApproveJobSeekersCommandResponse>>
  {
    [Required(ErrorMessage = "JobSeeker ID'leri gerekli")]
    [MinLength(1, ErrorMessage = "En az bir JobSeeker ID'si gerekli")]
    public List<string> JobSeekerIds { get; set; } = new List<string>();

    [Required(ErrorMessage = "Status gerekli")]
    public JobSeekerStatus Status { get; set; }

    public string? Reason { get; set; } // Toplu işlem sebebi için
  }
}
