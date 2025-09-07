

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.Articles
{
  public class ArticlesDTO : BaseResponseDTO
  {
    // Makale ile ilgili temel bilgiler - Basic article information
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Excerpt { get; set; }
    public byte[]? Photo { get; set; }
    public byte[]? Thumbnail { get; set; }
    public string? PhotoContentType { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? CategoryId { get; set; }
    public bool IsFeatured { get; set; } = false;
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; } = 0;
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? Slug { get; set; }
    public string Icon { get; set; } = "article";

    // İlişkili veriler - Related data
    public string? AuthorName { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryDescription { get; set; }
  }
}
