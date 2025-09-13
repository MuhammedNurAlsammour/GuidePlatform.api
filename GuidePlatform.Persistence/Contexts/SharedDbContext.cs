using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.SharedApi;

namespace GuidePlatform.Persistence.Contexts
{
  // Shared schema (storeplatformh) için DbContext
  public class SharedDbContext : DbContext, ISharedDbContext
  {
    public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options)
    {
      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<SharedProvince> provinces { get; set; }
    public DbSet<SharedCountry> countries { get; set; }
    public DbSet<SharedDistrict> districts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Shared schema kullan
      modelBuilder.HasDefaultSchema("storeplatformh");

      // Province entity configuration
      modelBuilder.Entity<SharedProvince>(entity =>
      {
        entity.ToTable("provinces");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.ProvinceName).HasMaxLength(255);
        entity.Property(e => e.PhotoContentType).HasMaxLength(100);
      });

      // Country entity configuration
      modelBuilder.Entity<SharedCountry>(entity =>
      {
        entity.ToTable("countries");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.CountryName).HasMaxLength(255);
        entity.Property(e => e.PhotoContentType).HasMaxLength(100);
      });

      // District entity configuration
      modelBuilder.Entity<SharedDistrict>(entity =>
      {
        entity.ToTable("districts");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.DistrictName).HasMaxLength(255);
        entity.Property(e => e.ProvinceName).HasMaxLength(255);
        entity.Property(e => e.PhotoContentType).HasMaxLength(100);
      });
    }
  }
}
