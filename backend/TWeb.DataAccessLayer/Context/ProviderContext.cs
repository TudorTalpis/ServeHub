using Microsoft.EntityFrameworkCore;
using TWeb.Domain.Entities.Provider;

namespace TWeb.DataAccessLayer.Context;

public class ProviderContext : DbContext
{
    public DbSet<ProviderProfile> ProviderProfiles { get; set; }
    public DbSet<ProviderApplication> ProviderApplications { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<TimeOff> TimeOffs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (DbSession.ConnectionString != null)
        {
            optionsBuilder.UseNpgsql(DbSession.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // List<string> properties → native PostgreSQL text[] arrays.
        modelBuilder.Entity<ProviderProfile>(b =>
        {
            b.Property(e => e.CategoryIds).HasColumnType("text[]");
            b.Property(e => e.PendingCategoryNames).HasColumnType("text[]");
            b.Property(e => e.GalleryPhotos).HasColumnType("text[]");
        });

        modelBuilder.Entity<ProviderApplication>(b =>
        {
            b.Property(e => e.CategoryIds).HasColumnType("text[]");
            b.Property(e => e.GalleryPhotos).HasColumnType("text[]");
            b.Property(e => e.Status)
                .HasConversion(
                    v => ((int)v).ToString(),
                    v => Enum.Parse<TWeb.Domain.Enums.ApplicationStatus>(v));
        });
    }
}
