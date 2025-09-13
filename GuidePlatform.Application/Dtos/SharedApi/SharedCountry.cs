namespace GuidePlatform.Application.Dtos.SharedApi
{
  // Shared schema'daki Country entity'si için DTO
  public class SharedCountry
  {
    public Guid Id { get; set; }
    public string? CountryName { get; set; }
    public byte[]? Photo { get; set; }
    public byte[]? Thumbnail { get; set; }
    public string? PhotoContentType { get; set; }
    public DateTime CreatedAt { get; set; }
    public int IsActive { get; set; }
  }
}
