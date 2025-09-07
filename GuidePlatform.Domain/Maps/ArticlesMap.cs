using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Maps
{
  public class ArticlesMap : IEntityTypeConfiguration<ArticlesViewModel>
  {
    public void Configure(EntityTypeBuilder<ArticlesViewModel> builder)
    {
      builder.ToTable("articles", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("articles_pkey");

      // Primary key
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // Article specific fields - Makale özel alanları
      builder.Property(x => x.Title)
           .HasColumnName("title")
           .HasMaxLength(500)
           .IsRequired();

      builder.Property(x => x.Content)
           .HasColumnName("content");

      builder.Property(x => x.Excerpt)
           .HasColumnName("excerpt");

      builder.Property(x => x.Photo)
           .HasColumnName("photo");

      builder.Property(x => x.Thumbnail)
           .HasColumnName("thumbnail");

      builder.Property(x => x.PhotoContentType)
           .HasColumnName("photo_content_type")
           .HasMaxLength(50);

      builder.Property(x => x.AuthorId)
           .HasColumnName("author_id")
           .IsRequired();

      builder.Property(x => x.CategoryId)
           .HasColumnName("category_id");

      builder.Property(x => x.IsFeatured)
           .HasColumnName("is_featured")
           .HasDefaultValue(false);

      builder.Property(x => x.IsPublished)
           .HasColumnName("is_published")
           .HasDefaultValue(false);

      builder.Property(x => x.PublishedAt)
           .HasColumnName("published_at");

      builder.Property(x => x.ViewCount)
           .HasColumnName("view_count")
           .HasDefaultValue(0);

      builder.Property(x => x.SeoTitle)
           .HasColumnName("seo_title")
           .HasMaxLength(255);

      builder.Property(x => x.SeoDescription)
           .HasColumnName("seo_description")
           .HasMaxLength(500);

      builder.Property(x => x.Slug)
           .HasColumnName("slug")
           .HasMaxLength(255);

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("article")
           .IsRequired();

      // Base entity fields - Temel entity alanları
      builder.Property(x => x.CreateUserId)
           .HasColumnName("create_user_id");

      builder.Property(x => x.UpdateUserId)
           .HasColumnName("update_user_id");

      builder.Property(x => x.AuthUserId)
           .HasColumnName("auth_user_id");

      builder.Property(x => x.AuthCustomerId)
           .HasColumnName("auth_customer_id");

      builder.Property(x => x.RowCreatedDate)
           .HasColumnName("row_created_date")
           .HasDefaultValueSql("CURRENT_TIMESTAMP")
           .IsRequired();

      builder.Property(x => x.RowUpdatedDate)
           .HasColumnName("row_updated_date")
           .HasDefaultValueSql("CURRENT_TIMESTAMP")
           .IsRequired();

      builder.Property(x => x.RowIsActive)
           .HasColumnName("row_is_active")
           .HasDefaultValue(true)
           .IsRequired();

      builder.Property(x => x.RowIsDeleted)
           .HasColumnName("row_is_deleted")
           .HasDefaultValue(false)
           .IsRequired();

      // Unique constraint - Benzersiz kısıtlama
      builder.HasIndex(x => x.Slug)
           .IsUnique()
           .HasDatabaseName("articles_slug_key");

      // Indexes - İndeksler
      builder.HasIndex(x => x.AuthorId)
           .HasDatabaseName("idx_articles_author");

      builder.HasIndex(x => x.CategoryId)
           .HasDatabaseName("idx_articles_category");

      builder.HasIndex(x => x.IsPublished)
           .HasDatabaseName("idx_articles_published");

      builder.HasIndex(x => x.Slug)
           .HasDatabaseName("idx_articles_slug");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_articles_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_articles_update_user");
    }
  }
}
