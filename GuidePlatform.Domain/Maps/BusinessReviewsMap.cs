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
  public class BusinessReviewsMap : IEntityTypeConfiguration<BusinessReviewsViewModel>
  {
    public void Configure(EntityTypeBuilder<BusinessReviewsViewModel> builder)
    {
      builder.ToTable("business_reviews", "guideplatform");

      builder.HasKey(x => x.Id)
           .HasName("business_reviews_pkey");

      builder.Property(x => x.Id)
           .HasColumnName("id")
           .HasDefaultValueSql("gen_random_uuid()")
           .IsRequired();

      // İş yeri ID'si - zorunlu alan
      builder.Property(x => x.BusinessId)
           .HasColumnName("business_id")
           .IsRequired();

      // Yorum yapan kullanıcı ID'si - zorunlu alan
      builder.Property(x => x.ReviewerId)
           .HasColumnName("reviewer_id")
           .IsRequired();

      // Değerlendirme puanı (1-5 arası) - zorunlu alan
      builder.Property(x => x.Rating)
           .HasColumnName("rating")
           .IsRequired();

      // Yorum metni - opsiyonel
      builder.Property(x => x.Comment)
           .HasColumnName("comment");

      // Yorum doğrulanmış mı? - varsayılan false
      builder.Property(x => x.IsVerified)
           .HasColumnName("is_verified")
           .HasDefaultValue(false);

      // Yorum onaylanmış mı? - varsayılan true
      builder.Property(x => x.IsApproved)
           .HasColumnName("is_approved")
           .HasDefaultValue(true);

      // İkon adı - varsayılan 'rate_review'
      builder.Property(x => x.Icon)
           .HasColumnName("icon")
           .HasMaxLength(100)
           .HasDefaultValue("rate_review");

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

      // Foreign key ilişkileri
      builder.HasOne<BusinessesViewModel>()
           .WithMany()
           .HasForeignKey(x => x.BusinessId)
           .OnDelete(DeleteBehavior.Cascade);

      // Index'ler
      builder.HasIndex(x => x.BusinessId)
           .HasDatabaseName("idx_business_reviews_business");

      builder.HasIndex(x => x.ReviewerId)
           .HasDatabaseName("idx_business_reviews_reviewer");

      builder.HasIndex(x => x.IsApproved)
           .HasDatabaseName("idx_business_reviews_approved");

      builder.HasIndex(x => x.CreateUserId)
           .HasDatabaseName("idx_business_reviews_create_user");

      builder.HasIndex(x => x.UpdateUserId)
           .HasDatabaseName("idx_business_reviews_update_user");
    }
  }
}
