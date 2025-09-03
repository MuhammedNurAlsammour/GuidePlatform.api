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
  public class BusinessesMap : IEntityTypeConfiguration<BusinessesViewModel>
  {
    public void Configure(EntityTypeBuilder<BusinessesViewModel> builder)
    {
      builder.ToTable("businesses", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("Businesses_pkey");

      // 🆔 Ana kimlik alanları - Primary identity fields
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // 👤 Kullanıcı kimlik alanları - User identity fields
      builder.Property(x => x.CreateUserId)
           .HasColumnName("create_user_id");

      builder.Property(x => x.UpdateUserId)
           .HasColumnName("update_user_id");

      builder.Property(x => x.AuthUserId)
           .HasColumnName("auth_user_id");

      builder.Property(x => x.AuthCustomerId)
           .HasColumnName("auth_customer_id");

      builder.Property(x => x.OwnerId)
           .HasColumnName("owner_id");

      // 🏢 Temel iş bilgileri - Basic business information
      builder.Property(x => x.Name)
           .HasColumnName("name")
           .HasMaxLength(255)
           .IsRequired();

      builder.Property(x => x.Description)
           .HasColumnName("description")
           .HasColumnType("text");

      builder.Property(x => x.CategoryId)
           .HasColumnName("category_id");

      builder.Property(x => x.SubCategoryId)
           .HasColumnName("sub_category_id");

      // 📍 Konum bilgileri - Location information
      builder.Property(x => x.ProvinceId)
           .HasColumnName("province_id");

      builder.Property(x => x.CountriesId)
           .HasColumnName("countries_id");

      builder.Property(x => x.DistrictId)
           .HasColumnName("district_id");

      builder.Property(x => x.Address)
           .HasColumnName("address")
           .HasColumnType("text");

      builder.Property(x => x.Latitude)
           .HasColumnName("latitude")
           .HasColumnType("numeric(10,8)");

      builder.Property(x => x.Longitude)
           .HasColumnName("longitude")
           .HasColumnType("numeric(11,8)");

      // 📞 İletişim bilgileri - Contact information
      builder.Property(x => x.Phone)
           .HasColumnName("phone")
           .HasMaxLength(20);

      builder.Property(x => x.Mobile)
           .HasColumnName("mobile")
           .HasMaxLength(20);

      builder.Property(x => x.Email)
           .HasColumnName("email")
           .HasMaxLength(255);

      builder.Property(x => x.Website)
           .HasColumnName("website")
           .HasMaxLength(500);

      builder.Property(x => x.FacebookUrl)
           .HasColumnName("facebook_url")
           .HasMaxLength(500);

      builder.Property(x => x.InstagramUrl)
           .HasColumnName("instagram_url")
           .HasMaxLength(500);

      builder.Property(x => x.WhatsApp)
           .HasColumnName("whatsapp")
           .HasMaxLength(20);

      builder.Property(x => x.Telegram)
           .HasColumnName("telegram")
           .HasMaxLength(100);

      // ⭐ Değerlendirme ve istatistikler - Rating and statistics
      builder.Property(x => x.Rating)
           .HasColumnName("rating")
           .HasColumnType("numeric(3,2)")
           .HasDefaultValue(0.00m);

      builder.Property(x => x.TotalReviews)
           .HasColumnName("total_reviews")
           .HasDefaultValue(0);

      builder.Property(x => x.ViewCount)
           .HasColumnName("view_count")
           .HasDefaultValue(0);

      // 💼 İş özellikleri - Business features
      builder.Property(x => x.SubscriptionType)
           .HasColumnName("subscription_type")
           .HasDefaultValue(0);

      builder.Property(x => x.IsVerified)
           .HasColumnName("is_verified")
           .HasDefaultValue(false);

      builder.Property(x => x.IsFeatured)
           .HasColumnName("is_featured")
           .HasDefaultValue(false);

      builder.Property(x => x.WorkingHours)
           .HasColumnName("working_hours")
           .HasColumnType("text");

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("business");

      // 📅 Sistem alanları - System fields
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

      // 🔗 İlişki tanımları - Relationship definitions
      builder.HasOne<BusinessesViewModel>()
           .WithMany()
           .HasForeignKey(x => x.CategoryId)
           .HasConstraintName("businesses_category_id_fkey");

      builder.HasOne<BusinessesViewModel>()
           .WithMany()
           .HasForeignKey(x => x.SubCategoryId)
           .HasConstraintName("businesses_sub_category_id_fkey");
    }
  }
}
