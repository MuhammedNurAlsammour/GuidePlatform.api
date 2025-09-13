using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Files.GetAllFiles
{
  /// <summary>
  /// Files için filtreleme ve sayfalama parametreleri
  /// </summary>
  public class GetAllFilesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllFilesQueryResponse>>
  {
    /// <summary>
    /// Dosya türü filtresi - File type filter
    /// 1: Resim, 2: Doküman, 3: Video, 4: Ses, 5: Arşiv
    /// </summary>
    public int? FileType { get; set; }

    /// <summary>
    /// Genel erişim filtresi - Public access filter
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>
    /// Dosya adı filtresi - File name filter (içerik arama)
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// MIME türü filtresi - MIME type filter
    /// </summary>
    public string? MimeType { get; set; }
  }
}
