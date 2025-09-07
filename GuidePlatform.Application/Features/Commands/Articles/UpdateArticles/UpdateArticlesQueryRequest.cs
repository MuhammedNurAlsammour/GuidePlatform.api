using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.ArticlesViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Articles.UpdateArticles
{
  public class UpdateArticlesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateArticlesCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // Makale ile ilgili güncellenebilir alanlar - Updatable article fields
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 500 characters")]
    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Excerpt { get; set; }

    public byte[]? Photo { get; set; }

    public byte[]? Thumbnail { get; set; }

    [StringLength(50, ErrorMessage = "Photo content type cannot exceed 50 characters")]
    public string? PhotoContentType { get; set; }

    public Guid? AuthorId { get; set; }

    public Guid? CategoryId { get; set; }

    public bool? IsFeatured { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public int? ViewCount { get; set; }

    [StringLength(255, ErrorMessage = "SEO title cannot exceed 255 characters")]
    public string? SeoTitle { get; set; }

    [StringLength(500, ErrorMessage = "SEO description cannot exceed 500 characters")]
    public string? SeoDescription { get; set; }

    [StringLength(255, ErrorMessage = "Slug cannot exceed 255 characters")]
    public string? Slug { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdateArticlesCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Makale alanlarını güncelle - Update article fields
      if (!string.IsNullOrWhiteSpace(request.Title))
        entity.Title = request.Title.Trim();

      if (!string.IsNullOrWhiteSpace(request.Content))
        entity.Content = request.Content.Trim();
      else if (request.Content == null)
        entity.Content = null;

      if (!string.IsNullOrWhiteSpace(request.Excerpt))
        entity.Excerpt = request.Excerpt.Trim();
      else if (request.Excerpt == null)
        entity.Excerpt = null;

      if (request.Photo != null)
        entity.Photo = request.Photo;

      if (request.Thumbnail != null)
        entity.Thumbnail = request.Thumbnail;

      if (!string.IsNullOrWhiteSpace(request.PhotoContentType))
        entity.PhotoContentType = request.PhotoContentType.Trim();
      else if (request.PhotoContentType == null)
        entity.PhotoContentType = null;

      if (request.AuthorId.HasValue)
        entity.AuthorId = request.AuthorId.Value;

      if (request.CategoryId.HasValue)
        entity.CategoryId = request.CategoryId.Value;
      else if (request.CategoryId == null)
        entity.CategoryId = null;

      if (request.IsFeatured.HasValue)
        entity.IsFeatured = request.IsFeatured.Value;

      if (request.IsPublished.HasValue)
        entity.IsPublished = request.IsPublished.Value;

      if (request.PublishedAt.HasValue)
        entity.PublishedAt = request.PublishedAt.Value;
      else if (request.PublishedAt == null)
        entity.PublishedAt = null;

      if (request.ViewCount.HasValue)
        entity.ViewCount = request.ViewCount.Value;

      if (!string.IsNullOrWhiteSpace(request.SeoTitle))
        entity.SeoTitle = request.SeoTitle.Trim();
      else if (request.SeoTitle == null)
        entity.SeoTitle = null;

      if (!string.IsNullOrWhiteSpace(request.SeoDescription))
        entity.SeoDescription = request.SeoDescription.Trim();
      else if (request.SeoDescription == null)
        entity.SeoDescription = null;

      if (!string.IsNullOrWhiteSpace(request.Slug))
        entity.Slug = request.Slug.Trim();
      else if (request.Slug == null)
        entity.Slug = null;

      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
