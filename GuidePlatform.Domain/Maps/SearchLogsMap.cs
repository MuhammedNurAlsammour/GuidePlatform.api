using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Maps
{
  public class SearchLogsMap : IEntityTypeConfiguration<SearchLogsViewModel>
  {
    public void Configure(EntityTypeBuilder<SearchLogsViewModel> builder)
    {
      builder.ToTable("search_logs", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("search_logs_pkey");

      // Primary key
      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // Search specific fields - Arama özel alanları
      builder.Property(x => x.SearchTerm)
           .HasColumnName("search_term")
           .HasMaxLength(255)
           .IsRequired();

      builder.Property(x => x.SearchFilters)
           .HasColumnName("search_filters")
           .HasColumnType("jsonb");

      builder.Property(x => x.ResultsCount)
           .HasColumnName("results_count");

      builder.Property(x => x.SearchDate)
           .HasColumnName("search_date")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");

      // Ignore the IPAddress property to avoid EF mapping conflicts - IPAddress property'sini EF mapping çakışmalarını önlemek için ignore et
      builder.Ignore(x => x.IpAddress);

      builder.Property(x => x.IpAddressString)
           .HasColumnName("ip_address")
           .HasColumnType("varchar(45)"); // IPv6 için maksimum uzunluk - Maximum length for IPv6

      builder.Property(x => x.UserAgent)
           .HasColumnName("user_agent");

      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("search")
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
      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_search_logs_create_user");

      builder.HasIndex(x => x.SearchDate)
           .HasDatabaseName("idx_search_logs_search_date");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_search_logs_update_user");

      builder.HasIndex(x => x.AuthUserId)
           .HasDatabaseName("idx_search_logs_user_id");
    }
  }
}
