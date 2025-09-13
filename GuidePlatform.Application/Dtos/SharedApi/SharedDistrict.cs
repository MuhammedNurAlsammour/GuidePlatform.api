namespace GuidePlatform.Application.Dtos.SharedApi
{
    // Shared schema'daki District entity'si için DTO
    public class SharedDistrict
    {
        public Guid Id { get; set; }
        public string? DistrictName { get; set; }
        public Guid? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public byte[]? Photo { get; set; }
        public byte[]? Thumbnail { get; set; }
        public string? PhotoContentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public int IsActive { get; set; }
    }
}
