namespace GuidePlatform.Application.Dtos.SharedApi
{
  // Shared schema'daki Province entity'si için DTO
  public class SharedProvince
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
