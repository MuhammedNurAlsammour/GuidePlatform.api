

namespace GuidePlatform.Application.Dtos.ResponseDtos.categories
{
	/// <summary>
	/// Kategori yanıt DTO'su
	/// </summary>
	public class categoriesDTO : BaseResponseDTO
    {
		/// <summary>
		/// Kategori adı
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Kategori açıklaması
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Üst kategori kimliği (alt kategoriler için)
		/// </summary>
		public Guid? ParentId { get; set; }

		/// <summary>
		/// Üst kategori adı
		/// </summary>
		public string? ParentName { get; set; }

		/// <summary>
		/// Kategori simgesi (Material Icons)
		/// </summary>
		public string Icon { get; set; } = "category";

		/// <summary>
		/// Sıralama düzeni
		/// </summary>
		public int SortOrder { get; set; } = 0;

		/// <summary>
		/// Alt kategori sayısı
		/// </summary>
		public int ChildrenCount { get; set; } = 0;

		/// <summary>
		/// Kategori tam yolu (parent/child)
		/// </summary>
		public string FullPath { get; set; } = string.Empty;

		/// <summary>
		/// Alt kategoriler
		/// </summary>
		public List<categoriesDTO> Children { get; set; } = new List<categoriesDTO>();
	}
}
