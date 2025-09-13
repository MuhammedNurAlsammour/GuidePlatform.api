namespace GuidePlatform.Application.Dtos.SharedApi
{
  // Shared API'den gelen Province bilgileri için DTO
  public class ProvinceDto
  {
    public Guid Id { get; set; }
    public string? ProvinceName { get; set; }
    public Guid? CountryId { get; set; }
    // CountryName yok - sadece CountryId var
    public byte[]? Photo { get; set; }
    public byte[]? Thumbnail { get; set; }
    public string? PhotoContentType { get; set; }
    public DateTime CreatedAt { get; set; }
    public int IsActive { get; set; }
  }
}
