using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.SharedApi;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Application.Services
{
  // Shared schema ile iletişim için service implementation'ı
  public class SharedApiService : ISharedApiService
  {
    private readonly ISharedDbContext _sharedDbContext;
    private readonly ILogger<SharedApiService> _logger;

    public SharedApiService(ISharedDbContext sharedDbContext, ILogger<SharedApiService> logger)
    {
      _sharedDbContext = sharedDbContext;
      _logger = logger;
    }

    // Tek bir Province bilgisini ID'ye göre getir
    public async Task<ProvinceDto?> GetProvinceByIdAsync(Guid provinceId, CancellationToken cancellationToken = default)
    {
      try
      {
        _logger.LogInformation("Province bilgisi getiriliyor. ProvinceId: {ProvinceId}", provinceId);

        var province = await _sharedDbContext.provinces
            .Where(p => p.Id == provinceId && p.IsActive == 1)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (province != null)
        {
          var provinceDto = new ProvinceDto
          {
            Id = province.Id,
            ProvinceName = province.ProvinceName,
            CountryId = province.CountryId,
            Photo = province.Photo,
            Thumbnail = province.Thumbnail,
            PhotoContentType = province.PhotoContentType,
            CreatedAt = province.CreatedAt,
            IsActive = province.IsActive
          };

          _logger.LogInformation("Province bilgisi başarıyla getirildi. ProvinceId: {ProvinceId}, ProvinceName: {ProvinceName}",
              provinceId, provinceDto.ProvinceName);

          return provinceDto;
        }

        _logger.LogWarning("Province bulunamadı. ProvinceId: {ProvinceId}", provinceId);
        return null;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Province bilgisi getirilirken hata oluştu. ProvinceId: {ProvinceId}", provinceId);
        return null;
      }
    }

    // Birden fazla Province bilgisini ID'lere göre getir
    public async Task<Dictionary<Guid, ProvinceDto>> GetProvincesByIdsAsync(List<Guid> provinceIds, CancellationToken cancellationToken = default)
    {
      var result = new Dictionary<Guid, ProvinceDto>();

      if (!provinceIds.Any())
      {
        return result;
      }

      try
      {
        _logger.LogInformation("Birden fazla Province bilgisi getiriliyor. ProvinceIds: {ProvinceIds}",
            string.Join(", ", provinceIds));

        var provinces = await _sharedDbContext.provinces
            .Where(p => provinceIds.Contains(p.Id) && p.IsActive == 1)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var province in provinces)
        {
          var provinceDto = new ProvinceDto
          {
            Id = province.Id,
            ProvinceName = province.ProvinceName,
            CountryId = province.CountryId,
            Photo = province.Photo,
            Thumbnail = province.Thumbnail,
            PhotoContentType = province.PhotoContentType,
            CreatedAt = province.CreatedAt,
            IsActive = province.IsActive
          };

          result[province.Id] = provinceDto;
        }

        _logger.LogInformation("Province bilgileri getirildi. Toplam: {Count}", result.Count);

        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Province bilgileri getirilirken hata oluştu. ProvinceIds: {ProvinceIds}",
            string.Join(", ", provinceIds));
        return result;
      }
    }
  }
}
