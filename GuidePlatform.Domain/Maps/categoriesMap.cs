using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Domain.Maps
{
	/// <summary>
	/// Ana ve Alt Kategoriler için Entity Configuration
	/// </summary>
	public class CategoriesMap : IEntityTypeConfiguration<CategoriesViewModel>
	{
		public void Configure(EntityTypeBuilder<CategoriesViewModel> builder)
		{
			builder.ToTable("categories", "guideplatform");

			builder.HasKey(x => x.Id)
				   .HasName("categories_pkey");

			builder.Property(x => x.Id)
				   .HasColumnName("id")
				   .HasDefaultValueSql("gen_random_uuid()")
				   .IsRequired();

			builder.Property(x => x.Name)
				   .HasColumnName("name")
				   .HasMaxLength(255)
				   .IsRequired();

			builder.Property(x => x.Description)
				   .HasColumnName("description")
				   .HasColumnType("text");

			builder.Property(x => x.ParentId)
				   .HasColumnName("parent_id");

			builder.Property(x => x.Icon)
				   .HasColumnName("icon")
				   .HasMaxLength(100)
				   .HasDefaultValue("category");

			builder.Property(x => x.SortOrder)
				   .HasColumnName("sort_order")
				   .HasDefaultValue(0);

			builder.Property(x => x.AuthUserId)
				   .HasColumnName("auth_user_id");

			builder.Property(x => x.AuthCustomerId)
				   .HasColumnName("auth_customer_id");

			builder.Property(x => x.CreateUserId)
				   .HasColumnName("create_user_id");

			builder.Property(x => x.UpdateUserId)
				   .HasColumnName("update_user_id");

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

			// Foreign Key Relationships
			builder.HasOne(x => x.Parent)
				   .WithMany(x => x.Children)
				   .HasForeignKey(x => x.ParentId)
				   .HasConstraintName("categories_parent_id_fkey")
				   .OnDelete(DeleteBehavior.Restrict);

			// Indexes
			builder.HasIndex(x => new { x.RowIsActive, x.RowIsDeleted })
				   .HasDatabaseName("idx_categories_active_deleted");

			builder.HasIndex(x => x.AuthCustomerId)
				   .HasDatabaseName("idx_categories_auth_customer")
				   .HasFilter("\"auth_customer_id\" IS NOT NULL");

			builder.HasIndex(x => x.AuthUserId)
				   .HasDatabaseName("idx_categories_auth_user")
				   .HasFilter("\"auth_user_id\" IS NOT NULL");

			builder.HasIndex(x => x.RowCreatedDate)
				   .HasDatabaseName("idx_categories_created_date");

			builder.HasIndex(x => x.ParentId)
				   .HasDatabaseName("idx_categories_parent_id")
				   .HasFilter("\"parent_id\" IS NOT NULL");

			builder.HasIndex(x => x.CreateUserId)
				   .HasDatabaseName("idx_categories_create_user");

			builder.HasIndex(x => x.UpdateUserId)
				   .HasDatabaseName("idx_categories_update_user");
		}
	}
}
