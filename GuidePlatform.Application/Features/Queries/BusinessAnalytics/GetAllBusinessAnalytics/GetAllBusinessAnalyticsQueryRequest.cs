using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetAllBusinessAnalytics
{
  public class GetAllBusinessAnalyticsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllBusinessAnalyticsQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    /// <summary>
    /// İş ID'sine göre filtreleme - Filter by Business ID
    /// </summary>
    public Guid? BusinessId { get; set; }

    /// <summary>
    /// Başlangıç tarihi - Start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Bitiş tarihi - End date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Minimum görüntülenme sayısı - Minimum views count
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum görüntülenme sayısı sıfır veya pozitif olmalıdır")]
    public int? MinViewsCount { get; set; }

    /// <summary>
    /// Maksimum görüntülenme sayısı - Maximum views count
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maksimum görüntülenme sayısı sıfır veya pozitif olmalıdır")]
    public int? MaxViewsCount { get; set; }

    /// <summary>
    /// Minimum iletişim sayısı - Minimum contacts count
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum iletişim sayısı sıfır veya pozitif olmalıdır")]
    public int? MinContactsCount { get; set; }

    /// <summary>
    /// Maksimum iletişim sayısı - Maximum contacts count
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maksimum iletişim sayısı sıfır veya pozitif olmalıdır")]
    public int? MaxContactsCount { get; set; }

    /// <summary>
    /// Minimum yorum sayısı - Minimum reviews count
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum yorum sayısı sıfır veya pozitif olmalıdır")]
    public int? MinReviewsCount { get; set; }

    /// <summary>
    /// Maksimum yorum sayısı - Maximum reviews count
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maksimum yorum sayısı sıfır veya pozitif olmalıdır")]
    public int? MaxReviewsCount { get; set; }

    /// <summary>
    /// Minimum favori sayısı - Minimum favorites count
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Minimum favori sayısı sıfır veya pozitif olmalıdır")]
    public int? MinFavoritesCount { get; set; }

    /// <summary>
    /// Maksimum favori sayısı - Maximum favorites count
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Maksimum favori sayısı sıfır veya pozitif olmalıdır")]
    public int? MaxFavoritesCount { get; set; }

    /// <summary>
    /// İkon türüne göre filtreleme - Filter by icon type
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Oluşturma kullanıcı ID'sine göre filtreleme - Filter by create user ID
    /// </summary>
    public Guid? CreateUserId { get; set; }

    /// <summary>
    /// Güncelleme kullanıcı ID'sine göre filtreleme - Filter by update user ID
    /// </summary>
    public Guid? UpdateUserId { get; set; }

    /// <summary>
    /// Sadece aktif kayıtları getir - Get only active records
    /// </summary>
    public bool? OnlyActive { get; set; } = true;

    /// <summary>
    /// Sadece silinmemiş kayıtları getir - Get only non-deleted records
    /// </summary>
    public bool? OnlyNonDeleted { get; set; } = true;
  }
}
