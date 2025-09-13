using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.FilesViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.Files.CreateFiles
{
  public class CreateFilesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateFilesCommandResponse>>
  {
    // Dosya ile ilgili temel bilgiler - Basic file information
    [Required(ErrorMessage = "File name is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "File name must be between 1 and 255 characters")]
    public string FileName { get; set; } = string.Empty;

    [Required(ErrorMessage = "File path is required")]
    public byte[] FilePath { get; set; } = Array.Empty<byte>();

    [Range(0, long.MaxValue, ErrorMessage = "File size must be a positive number")]
    public long? FileSize { get; set; }

    [StringLength(100, ErrorMessage = "MIME type cannot exceed 100 characters")]
    public string? MimeType { get; set; }

    public int? FileType { get; set; }

    public bool IsPublic { get; set; } = false;

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "file_copy";

    public static O Map(CreateFilesCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        // Auth bilgilerini sadece geçerli olduğunda ata - Assign auth info only when valid
        AuthCustomerId = customerId, // Null olabilir - Can be null
        AuthUserId = userId, // Null olabilir - Can be null
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId
        FileName = request.FileName,
        FilePath = request.FilePath,
        FileSize = request.FileSize,
        MimeType = request.MimeType,
        FileType = request.FileType,
        IsPublic = request.IsPublic,
        Icon = request.Icon,
      };
    }
  }
}

