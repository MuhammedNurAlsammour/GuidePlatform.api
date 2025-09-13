using GuidePlatform.Application.Dtos.SharedApi;

namespace GuidePlatform.Application.Abstractions.Services
{
  // Shared API ile iletişim için service interface'i
  public interface ISharedApiService
  {
    // Province bilgilerini ID'ye göre getir
    Task<ProvinceDto?> GetProvinceByIdAsync(Guid provinceId, CancellationToken cancellationToken = default);

    // Birden fazla Province bilgisini ID'lere göre getir
    Task<Dictionary<Guid, ProvinceDto>> GetProvincesByIdsAsync(List<Guid> provinceIds, CancellationToken cancellationToken = default);
  }
}
