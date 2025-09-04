using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetAllDropboxesBusinessImages
{
	public class GetAllDropboxesBusinessImagesQueryResponse
	{
		public List<businessImagesDetailDto> businessImages { get; set; } = new List<businessImagesDetailDto>();
	}
	
	public class businessImagesDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
        // İşletme ID'si - business_id
        public Guid BusinessId { get; set; }

        // Fotoğraf verisi - photo (genellikle API'de base64 string olarak gönderilir)
        public string? Photo { get; set; }

        // Küçük resim verisi - thumbnail (genellikle API'de base64 string olarak gönderilir)
        public string? Thumbnail { get; set; }

        // Fotoğraf içerik tipi - photo_content_type
        public string? PhotoContentType { get; set; }

        // Alternatif metin - alt_text
        public string? AltText { get; set; }

        // Ana fotoğraf mı - is_primary
        public bool IsPrimary { get; set; }

        // Sıralama düzeni - sort_order
        public int SortOrder { get; set; }

        // İkon - icon
        public string? Icon { get; set; }

        // Resim tipi - image_type
        public int ImageType { get; set; }

        // İşletme adı (join ile gelecek)
        public string? BusinessName { get; set; }
    }
}
