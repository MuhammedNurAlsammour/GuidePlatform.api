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
  public class PaymentsMap : IEntityTypeConfiguration<PaymentsViewModel>
  {
    public void Configure(EntityTypeBuilder<PaymentsViewModel> builder)
    {
      builder.ToTable("payments", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("payments_pkey");

      // Primary key
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // Payment specific fields - Ödeme özel alanları
      builder.Property(x => x.SubscriptionId)
           .HasColumnName("subscription_id")
           .IsRequired();

      builder.Property(x => x.Amount)
           .HasColumnName("amount")
           .HasColumnType("numeric(12,2)")
           .IsRequired();

      builder.Property(x => x.Currency)
           .HasColumnName("currency")
           .HasMaxLength(3)
           .HasDefaultValue("SYP")
           .IsRequired();

      builder.Property(x => x.PaymentMethod)
           .HasColumnName("payment_method")
           .HasMaxLength(50);

      builder.Property(x => x.TransactionId)
           .HasColumnName("transaction_id")
           .HasMaxLength(255);

      builder.Property(x => x.PaymentDate)
           .HasColumnName("payment_date")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");

      builder.Property(x => x.Status)
           .HasColumnName("status")
           .HasMaxLength(20)
           .HasDefaultValue("pending")
           .IsRequired();

      builder.Property(x => x.Notes)
           .HasColumnName("notes");

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("payment")
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

      // Foreign key relationships - Yabancı anahtar ilişkileri
      builder.HasOne(x => x.Subscription)
           .WithMany()
           .HasForeignKey(x => x.SubscriptionId)
           .OnDelete(DeleteBehavior.Cascade)
           .HasConstraintName("payments_subscription_id_fkey");

      // Indexes - İndeksler
      builder.HasIndex(x => x.SubscriptionId)
           .HasDatabaseName("idx_payments_subscription_id");

      builder.HasIndex(x => x.PaymentDate)
           .HasDatabaseName("idx_payments_payment_date");

      builder.HasIndex(x => x.Status)
           .HasDatabaseName("idx_payments_status");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_payments_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_payments_update_user");
    }
  }
}
