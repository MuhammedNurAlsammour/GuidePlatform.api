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
  public class BusinessImagesMap : IEntityTypeConfiguration<BusinessImagesViewModel>
  {
    public void Configure(EntityTypeBuilder<BusinessImagesViewModel> builder)
    {
      builder.ToTable("business_images", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("business_images_pkey");

      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // İşletme ID'si - business_id
      builder.Property(x => x.BusinessId)
           .HasColumnName("business_id")
           .IsRequired();

      // Fotoğraf verisi - photo
      builder.Property(x => x.Photo)
           .HasColumnName("photo");

      // Küçük resim verisi - thumbnail
      builder.Property(x => x.Thumbnail)
           .HasColumnName("thumbnail");

      // Fotoğraf URL'si - photo_url (yeni sistem)
      builder.Property(x => x.PhotoUrl)
           .HasColumnName("photo_url")
           .HasMaxLength(500);

      // Küçük resim URL'si - thumbnail_url (yeni sistem)
      builder.Property(x => x.ThumbnailUrl)
           .HasColumnName("thumbnail_url")
           .HasMaxLength(500);

      // Fotoğraf içerik tipi - photo_content_type
      builder.Property(x => x.PhotoContentType)
           .HasColumnName("photo_content_type")
           .HasMaxLength(50);

      // Alternatif metin - alt_text
      builder.Property(x => x.AltText)
           .HasColumnName("alt_text")
           .HasMaxLength(255);

      // Ana fotoğraf mı - is_primary
      builder.Property(x => x.IsPrimary)
           .HasColumnName("is_primary")
           .HasDefaultValue(false);

      // Sıralama düzeni - sort_order
      builder.Property(x => x.SortOrder)
           .HasColumnName("sort_order")
           .HasDefaultValue(0);

      // İkon - icon
      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("image");

      // Resim tipi - image_type
      builder.Property(x => x.ImageType)
           .HasColumnName("image_type")
           .HasDefaultValue(1);

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

      // Foreign key ilişkileri
      builder.HasOne<BusinessesViewModel>()
           .WithMany()
           .HasForeignKey(x => x.BusinessId)
           .OnDelete(DeleteBehavior.Cascade);

      // Index'ler
      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_business_images_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_business_images_update_user");

      // Check constraint - image_type değerleri
      builder.HasCheckConstraint("chk_image_type_valid",
        "image_type = ANY (ARRAY[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11])");
    }
  }
}
