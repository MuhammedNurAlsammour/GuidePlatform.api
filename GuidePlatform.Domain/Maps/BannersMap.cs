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
  public class BannersMap : IEntityTypeConfiguration<BannersViewModel>
  {
    public void Configure(EntityTypeBuilder<BannersViewModel> builder)
    {
      builder.ToTable("banners", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("banners_pkey");

      // Primary key
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // Banner specific fields - Banner özel alanları
      builder.Property(x => x.Title)
           .HasColumnName("title")
           .HasMaxLength(255)
           .IsRequired();

      builder.Property(x => x.ProvinceId)
           .HasColumnName("province_id");


      builder.Property(x => x.Description)
           .HasColumnName("description");

      builder.Property(x => x.Photo)
           .HasColumnName("photo");

      builder.Property(x => x.Thumbnail)
           .HasColumnName("thumbnail");

      // Yeni sistem: URL alanları - New system: URL fields
      builder.Property(x => x.PhotoUrl)
           .HasColumnName("photo_url")
           .HasMaxLength(500);

      builder.Property(x => x.ThumbnailUrl)
           .HasColumnName("thumbnail_url")
           .HasMaxLength(500);

      builder.Property(x => x.PhotoContentType)
           .HasColumnName("photo_content_type")
           .HasMaxLength(50);

      builder.Property(x => x.LinkUrl)
           .HasColumnName("link_url")
           .HasMaxLength(500);

      builder.Property(x => x.StartDate)
           .HasColumnName("start_date")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");

      builder.Property(x => x.EndDate)
           .HasColumnName("end_date");

      builder.Property(x => x.IsActive)
           .HasColumnName("is_active")
           .HasDefaultValue(true);

      builder.Property(x => x.OrderIndex)
           .HasColumnName("order_index")
           .HasDefaultValue(0);

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("image")
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
      builder.HasIndex(x => x.IsActive)
           .HasDatabaseName("idx_banners_is_active");

      builder.HasIndex(x => x.StartDate)
           .HasDatabaseName("idx_banners_start_date");

      builder.HasIndex(x => x.EndDate)
           .HasDatabaseName("idx_banners_end_date");

      builder.HasIndex(x => x.OrderIndex)
           .HasDatabaseName("idx_banners_order_index");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_banners_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_banners_update_user");

      builder.HasIndex(x => x.ProvinceId)
           .HasDatabaseName("idx_banners_province");

      // Yeni sistem: URL indeksleri - New system: URL indexes
      builder.HasIndex(x => x.PhotoUrl)
           .HasDatabaseName("idx_banners_photo_url");

      builder.HasIndex(x => x.ThumbnailUrl)
           .HasDatabaseName("idx_banners_thumbnail_url");
    }
  }
}
