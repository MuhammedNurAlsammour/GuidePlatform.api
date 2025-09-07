using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class PagesViewModel : BaseEntity
  {
    // Sayfa ile ilgili temel bilgiler - Basic page information
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedDate { get; set; }
    public string Icon { get; set; } = "article";
  }
}