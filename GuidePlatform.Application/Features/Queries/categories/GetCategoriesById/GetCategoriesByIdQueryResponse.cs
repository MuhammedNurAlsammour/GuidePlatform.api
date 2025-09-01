using GuidePlatform.Application.Dtos.ResponseDtos;
using GuidePlatform.Application.Dtos.ResponseDtos.categories;
using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.categories.GetCategoriesById
{
	public class GetCategoriesByIdQueryResponse
	{
		public GetCategoriesByIdDetailDto categories { get; set; }
	}
	
	public class GetCategoriesByIdDetailDto : BaseResponseDTO
	{
		public Guid Id { get; set; } // Kategori benzersiz kimliği
		public string Name { get; set; } // Kategori adı
		public string Description { get; set; } // Kategori açıklaması
		public Guid? ParentId { get; set; } // Üst kategori kimliği (varsa)
		public string ParentName { get; set; } // Üst kategori adı
		public string Icon { get; set; } // Kategori ikonu
		public int SortOrder { get; set; } // Sıralama numarası
		public int ChildrenCount { get; set; } // Alt kategori sayısı
		public string FullPath { get; set; } // Kategorinin tam yolu
	}
}
