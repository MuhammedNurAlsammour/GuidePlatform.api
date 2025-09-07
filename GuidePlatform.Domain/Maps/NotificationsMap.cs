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
  public class NotificationsMap : IEntityTypeConfiguration<NotificationsViewModel>
  {
    public void Configure(EntityTypeBuilder<NotificationsViewModel> builder)
    {
      builder.ToTable("notifications", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("notifications_pkey");

      // Primary key
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // Notification specific fields - Bildirim özel alanları
      builder.Property(x => x.RecipientUserId)
           .HasColumnName("recipient_user_id")
           .IsRequired();

      builder.Property(x => x.Title)
           .HasColumnName("title")
           .HasMaxLength(255)
           .IsRequired();

      builder.Property(x => x.Message)
           .HasColumnName("message")
           .IsRequired();

      builder.Property(x => x.NotificationType)
           .HasColumnName("notification_type")
           .HasMaxLength(50)
           .HasDefaultValue("info")
           .IsRequired();

      builder.Property(x => x.IsRead)
           .HasColumnName("is_read")
           .HasDefaultValue(false);

      builder.Property(x => x.ReadDate)
           .HasColumnName("read_date");

      builder.Property(x => x.ActionUrl)
           .HasColumnName("action_url")
           .HasMaxLength(500);

      builder.Property(x => x.RelatedEntityId)
           .HasColumnName("related_entity_id");

      builder.Property(x => x.RelatedEntityType)
           .HasColumnName("related_entity_type")
           .HasMaxLength(50);

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("notifications")
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
      builder.HasIndex(x => x.RecipientUserId)
           .HasDatabaseName("idx_notifications_user_id");

      builder.HasIndex(x => x.IsRead)
           .HasDatabaseName("idx_notifications_is_read");

      builder.HasIndex(x => x.NotificationType)
           .HasDatabaseName("idx_notifications_notification_type");

      builder.HasIndex(x => x.AuthCustomerId)
           .HasDatabaseName("idx_notifications_customer_id");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_notifications_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_notifications_update_user");
    }
  }
}
