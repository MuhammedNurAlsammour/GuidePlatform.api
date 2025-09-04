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
  public class BusinessContactsMap : IEntityTypeConfiguration<BusinessContactsViewModel>
  {
    public void Configure(EntityTypeBuilder<BusinessContactsViewModel> builder)
    {
      builder.ToTable("business_contacts", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("business_contacts_pkey");

      // Ana ID alanı
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // İşletme ID'si - foreign key
      builder.Property(x => x.BusinessId)
           .HasColumnName("business_id")
           .IsRequired();

      // İletişim türü
      builder.Property(x => x.ContactType)
           .HasColumnName("contact_type")
           .HasMaxLength(50)
           .IsRequired();

      // İletişim değeri
      builder.Property(x => x.ContactValue)
           .HasColumnName("contact_value")
           .HasMaxLength(255)
           .IsRequired();

      // Birincil iletişim bilgisi
      builder.Property(x => x.IsPrimary)
           .HasColumnName("is_primary")
           .HasDefaultValue(false);

      // İkon türü
      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("contact_phone");

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
           .HasDatabaseName("idx_business_contacts_business_id");

      builder.HasIndex(x => x.ContactType)
           .HasDatabaseName("idx_business_contacts_contact_type");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_business_contacts_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_business_contacts_update_user");
    }
  }
}
