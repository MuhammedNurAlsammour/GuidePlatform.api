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
  public class BusinessAnalyticsMap : IEntityTypeConfiguration<BusinessAnalyticsViewModel>
  {
    public void Configure(EntityTypeBuilder<BusinessAnalyticsViewModel> builder)
    {
      builder.ToTable("business_analytics", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("business_analytics_pkey");

      // Unique constraint for business_id and date combination
      builder.HasIndex(x => new { x.BusinessId, x.Date })
           .IsUnique()
           .HasDatabaseName("business_analytics_business_id_date_key");

      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      builder.Property(x => x.BusinessId)
           .HasColumnName("business_id")
           .IsRequired();

      builder.Property(x => x.Date)
           .HasColumnName("date")
           .HasColumnType("date")
           .IsRequired();

      builder.Property(x => x.ViewsCount)
           .HasColumnName("views_count")
           .HasDefaultValue(0);

      builder.Property(x => x.ContactsCount)
           .HasColumnName("contacts_count")
           .HasDefaultValue(0);

      builder.Property(x => x.ReviewsCount)
           .HasColumnName("reviews_count")
           .HasDefaultValue(0);

      builder.Property(x => x.FavoritesCount)
           .HasColumnName("favorites_count")
           .HasDefaultValue(0);

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("analytics");

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
    }
  }
}
