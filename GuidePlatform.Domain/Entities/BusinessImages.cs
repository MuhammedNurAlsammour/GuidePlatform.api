using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class BusinessImagesViewModel : BaseEntity
  {
    // İşletme ID'si - business_id
    public Guid BusinessId { get; set; }

    // Fotoğraf verisi - photo
    public byte[]? Photo { get; set; }

    // Küçük resim verisi - thumbnail
    public byte[]? Thumbnail { get; set; }

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
  }
}