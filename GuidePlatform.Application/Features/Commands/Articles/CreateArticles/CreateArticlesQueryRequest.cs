using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.ArticlesViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.Articles.CreateArticles
{
  public class CreateArticlesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateArticlesCommandResponse>>
  {
    // Makale ile ilgili temel bilgiler - Basic article information
    [Required(ErrorMessage = "Title is required")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 500 characters")]
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string? Excerpt { get; set; }

    public byte[]? Photo { get; set; }

    public byte[]? Thumbnail { get; set; }

    [StringLength(50, ErrorMessage = "Photo content type cannot exceed 50 characters")]
    public string? PhotoContentType { get; set; }

    [Required(ErrorMessage = "Author ID is required")]
    public Guid AuthorId { get; set; }

    public Guid? CategoryId { get; set; }

    public bool IsFeatured { get; set; } = false;

    public bool IsPublished { get; set; } = false;

    public DateTime? PublishedAt { get; set; }

    public int ViewCount { get; set; } = 0;

    [StringLength(255, ErrorMessage = "SEO title cannot exceed 255 characters")]
    public string? SeoTitle { get; set; }

    [StringLength(500, ErrorMessage = "SEO description cannot exceed 500 characters")]
    public string? SeoDescription { get; set; }

    [StringLength(255, ErrorMessage = "Slug cannot exceed 255 characters")]
    public string? Slug { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "article";

    public static O Map(CreateArticlesCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId
        Title = request.Title,
        Content = request.Content,
        Excerpt = request.Excerpt,
        Photo = request.Photo,
        Thumbnail = request.Thumbnail,
        PhotoContentType = request.PhotoContentType,
        AuthorId = request.AuthorId,
        CategoryId = request.CategoryId,
        IsFeatured = request.IsFeatured,
        IsPublished = request.IsPublished,
        PublishedAt = request.PublishedAt,
        ViewCount = request.ViewCount,
        SeoTitle = request.SeoTitle,
        SeoDescription = request.SeoDescription,
        Slug = request.Slug,
        Icon = request.Icon,
      };
    }
  }
}

