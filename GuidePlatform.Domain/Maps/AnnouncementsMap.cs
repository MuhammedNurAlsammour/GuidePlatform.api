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
  public class AnnouncementsMap : IEntityTypeConfiguration<AnnouncementsViewModel>
  {
    public void Configure(EntityTypeBuilder<AnnouncementsViewModel> builder)
    {
      builder.ToTable("announcements", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("announcements_pkey");

      // Primary key
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // Announcement specific fields - Duyuru özel alanları
      builder.Property(x => x.Title)
           .HasColumnName("title")
           .HasMaxLength(255)
           .IsRequired();

      builder.Property(x => x.Content)
           .HasColumnName("content")
           .IsRequired();

      builder.Property(x => x.Priority)
           .HasColumnName("priority")
           .HasDefaultValue(1);

      builder.Property(x => x.IsPublished)
           .HasColumnName("is_published")
           .HasDefaultValue(false);

      builder.Property(x => x.PublishedDate)
           .HasColumnName("published_date");

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("announcement")
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

      // Indexes - İndeksler
      builder.HasIndex(x => x.IsPublished)
           .HasDatabaseName("idx_announcements_is_published");

      builder.HasIndex(x => x.Priority)
           .HasDatabaseName("idx_announcements_priority");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_announcements_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_announcements_update_user");
    }
  }
}
