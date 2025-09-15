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
  public class JobSeekersMap : IEntityTypeConfiguration<JobSeekersViewModel>
  {
    public void Configure(EntityTypeBuilder<JobSeekersViewModel> builder)
    {
      builder.ToTable("job_seekers", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("job_seekers_pkey");

      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      builder.Property(x => x.BusinessId)
           .HasColumnName("business_id");

      builder.Property(x => x.FullName)
           .HasColumnName("full_name")
           .HasMaxLength(255)
           .IsRequired();

      builder.Property(x => x.Description)
           .HasColumnName("description")
           .HasColumnType("text");

      builder.Property(x => x.Phone)
           .HasColumnName("phone")
           .HasMaxLength(20);

      builder.Property(x => x.IsSponsored)
           .HasColumnName("is_sponsored")
           .HasDefaultValue(false);

      builder.Property(x => x.ProvinceId)
           .HasColumnName("province_id");

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

      // İndeksler
      builder.HasIndex(x => new { x.RowIsActive, x.RowIsDeleted })
           .HasDatabaseName("idx_job_seekers_active");

      builder.HasIndex(x => x.AuthUserId)
           .HasDatabaseName("idx_job_seekers_auth_user_id");

      builder.HasIndex(x => x.BusinessId)
           .HasDatabaseName("idx_job_seekers_business_id");

      builder.HasIndex(x => x.IsSponsored)
           .HasDatabaseName("idx_job_seekers_sponsored");
    }
  }
}
