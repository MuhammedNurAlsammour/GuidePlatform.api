using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class FilesViewModel : BaseEntity
  {
    // Dosya ile ilgili temel bilgiler - Basic file information
    public string FileName { get; set; } = string.Empty;
    public byte[] FilePath { get; set; } = Array.Empty<byte>();
    public long? FileSize { get; set; }
    public string? MimeType { get; set; }
    public int? FileType { get; set; }
    public bool IsPublic { get; set; } = false;
    public string Icon { get; set; } = "file_copy";
  }
}