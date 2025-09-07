using MediatR;
using GuidePlatform.Application.Dtos.Response;

namespace GuidePlatform.Application.Features.Queries.Base
{
  /// <summary>
  /// OData query'leri için base request class
  /// Tüm OData parametrelerini içerir
  /// </summary>
  public abstract class BaseODataQueryRequest<TResponse> : IRequest<TransactionResultPack<TResponse>>
    where TResponse : class
  {
    /// <summary>
    /// OData filter parametresi
    /// Örnek: "Rating gt 3 and BusinessName contains 'Restaurant'"
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// OData orderby parametresi
    /// Örnek: "Rating desc, BusinessName asc"
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// OData select parametresi
    /// Örnek: "Id, BusinessName, Rating"
    /// </summary>
    public string? Select { get; set; }

    /// <summary>
    /// OData top parametresi (kaç kayıt alınacağı)
    /// </summary>
    public int? Top { get; set; }

    /// <summary>
    /// OData skip parametresi (kaç kayıt atlanacağı)
    /// </summary>
    public int? Skip { get; set; }

    /// <summary>
    /// OData count parametresi (toplam kayıt sayısı istenip istenmediği)
    /// </summary>
    public bool? Count { get; set; }

    /// <summary>
    /// Auth filtreleri için UserId
    /// </summary>
    public Guid? AuthUserId { get; set; }

    /// <summary>
    /// Auth filtreleri için CustomerId
    /// </summary>
    public Guid? AuthCustomerId { get; set; }

    /// <summary>
    /// Top değerini güvenli hale getirir (1-100 arası)
    /// </summary>
    public int GetValidatedTop()
    {
      if (!Top.HasValue) return 100; // Default değer
      return Math.Max(1, Math.Min(100, Top.Value));
    }

    /// <summary>
    /// Skip değerini güvenli hale getirir (0 veya pozitif)
    /// </summary>
    public int GetValidatedSkip()
    {
      if (!Skip.HasValue) return 0;
      return Math.Max(0, Skip.Value);
    }

    /// <summary>
    /// Filter'ın boş olup olmadığını kontrol eder
    /// </summary>
    public bool HasFilter => !string.IsNullOrWhiteSpace(Filter);

    /// <summary>
    /// OrderBy'nin boş olup olmadığını kontrol eder
    /// </summary>
    public bool HasOrderBy => !string.IsNullOrWhiteSpace(OrderBy);

    /// <summary>
    /// Select'in boş olup olmadığını kontrol eder
    /// </summary>
    public bool HasSelect => !string.IsNullOrWhiteSpace(Select);

    /// <summary>
    /// Count parametresinin true olup olmadığını kontrol eder
    /// </summary>
    public bool ShouldCount => Count.HasValue && Count.Value;
  }
}
