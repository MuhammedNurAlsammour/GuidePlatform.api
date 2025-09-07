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
  public class SubscriptionsMap : IEntityTypeConfiguration<SubscriptionsViewModel>
  {
    public void Configure(EntityTypeBuilder<SubscriptionsViewModel> builder)
    {
      builder.ToTable("subscriptions", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("subscriptions_pkey");

      // Ana kimlik alanı
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // İş kimliği - foreign key
      builder.Property(x => x.BusinessId)
           .HasColumnName("business_id")
           .IsRequired();

      // Başlangıç tarihi
      builder.Property(x => x.StartDate)
           .HasColumnName("start_date")
           .HasColumnType("date")
           .IsRequired();

      // Bitiş tarihi
      builder.Property(x => x.EndDate)
           .HasColumnName("end_date")
           .HasColumnType("date")
           .IsRequired();

      // Tutar
      builder.Property(x => x.Amount)
           .HasColumnName("amount")
           .HasColumnType("numeric(12,2)")
           .IsRequired();

      // Ödeme durumu
      builder.Property(x => x.PaymentStatus)
           .HasColumnName("payment_status")
           .HasDefaultValue(0);

      // İkon
      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("subscriptions");

      // Para birimi
      builder.Property(x => x.Currency)
           .HasColumnName("currency")
           .HasDefaultValue(1);

      // Durum
      builder.Property(x => x.Status)
           .HasColumnName("status")
           .HasDefaultValue(1);

      // Abonelik türü
      builder.Property(x => x.SubscriptionType)
           .HasColumnName("subscription_type")
           .IsRequired();

      // Kullanıcı kimlik alanları
      builder.Property(x => x.AuthUserId)
           .HasColumnName("auth_user_id");

      builder.Property(x => x.AuthCustomerId)
           .HasColumnName("auth_customer_id");

      builder.Property(x => x.CreateUserId)
           .HasColumnName("create_user_id");

      builder.Property(x => x.UpdateUserId)
           .HasColumnName("update_user_id");

      // Sistem alanları
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

      // Foreign key ilişkileri - Business ilişkisi
      // Not: Business entity'si henüz tanımlanmadığı için şimdilik comment'li bırakıyoruz
      // builder.HasOne<BusinessViewModel>()
      //      .WithMany()
      //      .HasForeignKey(x => x.BusinessId)
      //      .OnDelete(DeleteBehavior.Cascade);

      // İndeksler
      builder.HasIndex(x => x.BusinessId)
           .HasDatabaseName("idx_subscriptions_business_id");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_subscriptions_create_user");

      builder.HasIndex(x => x.EndDate)
           .HasDatabaseName("idx_subscriptions_end_date");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_subscriptions_update_user");
    }
  }
}
