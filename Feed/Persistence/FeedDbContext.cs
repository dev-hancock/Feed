using Feed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Feed.Persistence;

public class FeedDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Plan> Plans => Set<Plan>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Plan>(Configure);
        builder.Entity<Artifact>(Configure);
    }

    private static void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Url).IsRequired();
        builder.Property(e => e.Interval).IsRequired();
        builder.Property(e => e.IsActive).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.ChangedAt);
        builder.Property(e => e.CheckedAt);
        builder.Property(e => e.UpdatedAt);

        builder.HasMany(e => e.Artifacts)
            .WithOne(a => a.Plan)
            .HasForeignKey(a => a.FeedId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void Configure(EntityTypeBuilder<Artifact> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FeedId).IsRequired();

        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.ChangedAt);
        builder.Property(e => e.CheckedAt);
        builder.Property(e => e.UpdatedAt);

        builder.OwnsOne(e => e.Fields, Configure);
    }

    private static void Configure(OwnedNavigationBuilder<Artifact, Fields> builder)
    {
        builder.OwnsOne(e => e.Item, Configure);
        builder.OwnsOne(e => e.Title, Configure);
        builder.OwnsOne(e => e.Link, Configure);
        builder.OwnsOne(e => e.Description, Configure);
        builder.OwnsOne(e => e.Date, Configure);
    }

    private static void Configure(OwnedNavigationBuilder<Fields, Field> builder)
    {
        builder.Property(e => e.Selector).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Value).HasMaxLength(1000);
    }
}