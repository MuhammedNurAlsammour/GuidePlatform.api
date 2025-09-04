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
  public class UserVisitsMap : IEntityTypeConfiguration<UserVisitsViewModel>
  {
    public void Configure(EntityTypeBuilder<UserVisitsViewModel> builder)
    {
      builder.ToTable("user_visits", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("user_visits_pkey");

      // Ana ID alanı
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // İşletme ID'si - foreign key
      builder.Property(x => x.BusinessId)
           .HasColumnName("business_id")
           .IsRequired();

      // Ziyaret tarihi
      builder.Property(x => x.VisitDate)
           .HasColumnName("visit_date")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");

      // Ziyaret türü
      builder.Property(x => x.VisitType)
           .HasColumnName("visit_type")
           .HasMaxLength(50)
           .HasDefaultValue("view");

      // İkon türü
      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("visibility");

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

      // İndeksler
      builder.HasIndex(x => x.BusinessId)
           .HasDatabaseName("idx_user_visits_business_id");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_user_visits_create_user");

      builder.HasIndex(x => x.AuthCustomerId)
           .HasDatabaseName("idx_user_visits_customer_id");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_user_visits_update_user");

      builder.HasIndex(x => x.AuthUserId)
           .HasDatabaseName("idx_user_visits_user_id");

      builder.HasIndex(x => x.VisitDate)
           .HasDatabaseName("idx_user_visits_visit_date");
    }
  }
}
