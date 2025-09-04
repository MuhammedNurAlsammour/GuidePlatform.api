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
	public class BusinessServicesMap : IEntityTypeConfiguration<BusinessServicesViewModel>
	{
		public void Configure(EntityTypeBuilder<BusinessServicesViewModel> builder)
		{
			builder.ToTable("business_services", "guideplatform");

			builder.HasKey(x => x.Id)
				   .HasName("business_services_pkey");

			// Ana ID alanı
			builder.Property(x => x.Id)
				   .HasColumnName("id")
				   .HasDefaultValueSql("gen_random_uuid()")
				   .IsRequired();

			// İşletme ID'si - foreign key
			builder.Property(x => x.BusinessId)
				   .HasColumnName("business_id")
				   .IsRequired();

			// Hizmet adı
			builder.Property(x => x.ServiceName)
				   .HasColumnName("service_name")
				   .HasMaxLength(255)
				   .IsRequired();

			// Hizmet açıklaması
			builder.Property(x => x.ServiceDescription)
				   .HasColumnName("service_description")
				   .HasColumnType("text");

			// Fiyat
			builder.Property(x => x.Price)
				   .HasColumnName("price")
				   .HasColumnType("numeric(12,2)");

			// Para birimi
			builder.Property(x => x.Currency)
				   .HasColumnName("currency")
				   .HasMaxLength(3)
				   .HasDefaultValue("SYP");

			// Hizmet mevcut mu?
			builder.Property(x => x.IsAvailable)
				   .HasColumnName("is_available")
				   .HasDefaultValue(true);

			// İkon türü
			builder.Property(x => x.Icon)
				   .HasColumnName("icon")
				   .HasMaxLength(100)
				   .HasDefaultValue("miscellaneous_services");

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
				   .HasDatabaseName("idx_business_services_business_id");

			builder.HasIndex(x => x.CreateUserId)
				   .HasDatabaseName("idx_business_services_create_user");

			builder.HasIndex(x => x.IsAvailable)
				   .HasDatabaseName("idx_business_services_is_available");

			builder.HasIndex(x => x.UpdateUserId)
				   .HasDatabaseName("idx_business_services_update_user");
		}
	}
}
