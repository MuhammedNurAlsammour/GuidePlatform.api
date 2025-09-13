using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.FilesViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Files.UpdateFiles
{
  public class UpdateFilesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateFilesCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // Dosya ile ilgili güncellenebilir alanlar - Updatable file fields
    [StringLength(255, MinimumLength = 1, ErrorMessage = "File name must be between 1 and 255 characters")]
    public string? FileName { get; set; }

    public byte[]? FilePath { get; set; }

    [Range(0, long.MaxValue, ErrorMessage = "File size must be a positive number")]
    public long? FileSize { get; set; }

    [StringLength(100, ErrorMessage = "MIME type cannot exceed 100 characters")]
    public string? MimeType { get; set; }

    public int? FileType { get; set; }

    public bool? IsPublic { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdateFilesCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Dosya alanlarını güncelle - Update file fields
      if (!string.IsNullOrWhiteSpace(request.FileName))
        entity.FileName = request.FileName.Trim();

      if (request.FilePath != null)
        entity.FilePath = request.FilePath;

      if (request.FileSize.HasValue)
        entity.FileSize = request.FileSize.Value;

      if (!string.IsNullOrWhiteSpace(request.MimeType))
        entity.MimeType = request.MimeType.Trim();
      else if (request.MimeType == null)
        entity.MimeType = null;

      if (request.FileType.HasValue)
        entity.FileType = request.FileType.Value;
      else if (request.FileType == null)
        entity.FileType = null;

      if (request.IsPublic.HasValue)
        entity.IsPublic = request.IsPublic.Value;

      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
