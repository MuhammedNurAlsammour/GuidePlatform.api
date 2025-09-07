using GuidePlatform.Application.Dtos.ResponseDtos;

namespace GuidePlatform.Application.Features.Queries.Base
{
  /// <summary>
  /// OData query'leri için base response class
  /// Tüm OData response'ları için ortak özellikler
  /// </summary>
  public abstract class BaseODataQueryResponse<TDto> where TDto : BaseResponseDTO
  {
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Response mesajı
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Response timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Toplam kayıt sayısı
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// DTO listesi
    /// </summary>
    public List<TDto> Data { get; set; } = new();

    /// <summary>
    /// OData count değeri (eğer istenmişse)
    /// </summary>
    public int? Count { get; set; }

    /// <summary>
    /// Sayfalama bilgileri
    /// </summary>
    public PaginationInfo? Pagination { get; set; }

  }

  /// <summary>
  /// Sayfalama bilgileri
  /// </summary>
  public class PaginationInfo
  {
    /// <summary>
    /// Toplam sayfa sayısı
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Mevcut sayfa numarası
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Sayfa boyutu
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Toplam kayıt sayısı
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Önceki sayfa var mı?
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Sonraki sayfa var mı?
    /// </summary>
    public bool HasNextPage { get; set; }
  }
}
