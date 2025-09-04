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
  public class BusinessWorkingHoursMap : IEntityTypeConfiguration<BusinessWorkingHoursViewModel>
  {
    public void Configure(EntityTypeBuilder<BusinessWorkingHoursViewModel> builder)
    {
      builder.ToTable("business_working_hours", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("business_working_hours_pkey");

      // Ana ID alanı
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // İşletme ID'si - foreign key
      builder.Property(x => x.BusinessId)
           .HasColumnName("business_id")
           .IsRequired();

      // Haftanın günü
      builder.Property(x => x.DayOfWeek)
           .HasColumnName("day_of_week")
           .IsRequired();

      // Açılış saati
      builder.Property(x => x.OpenTime)
           .HasColumnName("open_time")
           .HasColumnType("time");

      // Kapanış saati
      builder.Property(x => x.CloseTime)
           .HasColumnName("close_time")
           .HasColumnType("time");

      // O gün kapalı mı?
      builder.Property(x => x.IsClosed)
           .HasColumnName("is_closed")
           .HasDefaultValue(false);

      // İkon türü
      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("schedule");

      // Kullanıcı bilgileri
      builder.Property(x => x.CreateUserId)
           .HasColumnName("create_user_id");

      builder.Property(x => x.UpdateUserId)
           .HasColumnName("update_user_id");

      builder.Property(x => x.AuthUserId)
           .HasColumnName("auth_user_id");

      builder.Property(x => x.AuthCustomerId)
           .HasColumnName("auth_customer_id");

      // Tarih alanları
      builder.Property(x => x.RowCreatedDate)
           .HasColumnName("row_created_date")
           .HasDefaultValueSql("CURRENT_TIMESTAMP")
           .IsRequired();

      builder.Property(x => x.RowUpdatedDate)
           .HasColumnName("row_updated_date")
           .HasDefaultValueSql("CURRENT_TIMESTAMP")
           .IsRequired();

      // Durum alanları
      builder.Property(x => x.RowIsActive)
           .HasColumnName("row_is_active")
           .HasDefaultValue(true)
           .IsRequired();

      builder.Property(x => x.RowIsDeleted)
           .HasColumnName("row_is_deleted")
           .HasDefaultValue(false)
           .IsRequired();

      // Check constraint
      builder.HasCheckConstraint("business_working_hours_day_of_week_check",
        "day_of_week >= 1 AND day_of_week <= 7");

      // İndeksler
      builder.HasIndex(x => x.BusinessId)
           .HasDatabaseName("idx_business_working_hours_business_id");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_business_working_hours_create_user");

      builder.HasIndex(x => x.DayOfWeek)
           .HasDatabaseName("idx_business_working_hours_day");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_business_working_hours_update_user");
    }
  }
}
