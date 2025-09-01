using GuidePlatform.Domain.Entities.Common;

namespace GuidePlatform.Domain.Entities
{
    public class CategoriesViewModel : BaseEntity
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
        /// Kategori simgesi (Material Icons)
        /// </summary>
        public string Icon { get; set; } = "category";

        /// <summary>
        /// Sıralama düzeni
        /// </summary>
        public int SortOrder { get; set; } = 0;

        // Navigation Properties
        /// <summary>
        /// Üst kategori
        /// </summary>
        public virtual CategoriesViewModel? Parent { get; set; }

        /// <summary>
        /// Alt kategoriler
        /// </summary>
        public virtual ICollection<CategoriesViewModel> Children { get; set; } = new List<CategoriesViewModel>();
    }
}