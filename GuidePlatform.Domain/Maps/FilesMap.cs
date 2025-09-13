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
  public class FilesMap : IEntityTypeConfiguration<FilesViewModel>
  {
    public void Configure(EntityTypeBuilder<FilesViewModel> builder)
    {
      builder.ToTable("files", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("files_pkey");

      // Primary key
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // File specific fields - Dosya özel alanları
      builder.Property(x => x.FileName)
           .HasColumnName("file_name")
           .HasMaxLength(255)
           .IsRequired();

      builder.Property(x => x.FilePath)
           .HasColumnName("file_path")
           .IsRequired();

      builder.Property(x => x.FileSize)
           .HasColumnName("file_size");

      builder.Property(x => x.MimeType)
           .HasColumnName("mime_type")
           .HasMaxLength(100);

      builder.Property(x => x.FileType)
           .HasColumnName("file_type");

      builder.Property(x => x.IsPublic)
           .HasColumnName("is_public")
           .HasDefaultValue(false);

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("file_copy")
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
      builder.HasIndex(x => x.FileType)
           .HasDatabaseName("idx_files_file_type");

      builder.HasIndex(x => x.IsPublic)
           .HasDatabaseName("idx_files_is_public");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_files_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_files_update_user");
    }
  }
}
