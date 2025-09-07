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
  public class PagesMap : IEntityTypeConfiguration<PagesViewModel>
  {
    public void Configure(EntityTypeBuilder<PagesViewModel> builder)
    {
      builder.ToTable("pages", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("pages_pkey");

      // Primary key
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // Page specific fields - Sayfa özel alanları
      builder.Property(x => x.Title)
           .HasColumnName("title")
           .HasMaxLength(255)
           .IsRequired();

      builder.Property(x => x.Slug)
           .HasColumnName("slug")
           .HasMaxLength(255)
           .IsRequired();

      builder.Property(x => x.Content)
           .HasColumnName("content");

      builder.Property(x => x.MetaDescription)
           .HasColumnName("meta_description");

      builder.Property(x => x.MetaKeywords)
           .HasColumnName("meta_keywords");

      builder.Property(x => x.IsPublished)
           .HasColumnName("is_published")
           .HasDefaultValue(false);

      builder.Property(x => x.PublishedDate)
           .HasColumnName("published_date");

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
           .HasDatabaseName("pages_slug_key");

      // Indexes - İndeksler
      builder.HasIndex(x => x.IsPublished)
           .HasDatabaseName("idx_pages_is_published");

      builder.HasIndex(x => x.Slug)
           .HasDatabaseName("idx_pages_slug");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_pages_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_pages_update_user");
    }
  }
}
