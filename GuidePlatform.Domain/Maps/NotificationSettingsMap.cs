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
  public class NotificationSettingsMap : IEntityTypeConfiguration<NotificationSettingsViewModel>
  {
    public void Configure(EntityTypeBuilder<NotificationSettingsViewModel> builder)
    {
      builder.ToTable("notification_settings", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("notification_settings_pkey");

      // Primary key
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // Notification settings specific fields - Bildirim ayarları özel alanları
      builder.Property(x => x.UserId)
           .HasColumnName("user_id")
           .IsRequired();

      builder.Property(x => x.SettingType)
           .HasColumnName("setting_type")
           .IsRequired();

      builder.Property(x => x.IsEnabled)
           .HasColumnName("is_enabled")
           .HasDefaultValue(true);

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("settings")
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
      builder.HasIndex(x => new { x.UserId, x.SettingType })
           .IsUnique()
           .HasDatabaseName("notification_settings_user_id_setting_type_key");

      // Indexes - İndeksler
      builder.HasIndex(x => x.UserId)
           .HasDatabaseName("idx_notification_settings_user_id");

      builder.HasIndex(x => x.AuthUserId)
           .HasDatabaseName("idx_notification_settings_auth_user_id");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_notification_settings_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_notification_settings_update_user");
    }
  }
}
