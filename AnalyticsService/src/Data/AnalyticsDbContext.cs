using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models.Domain;

namespace AnalyticsService.Data;

public class AnalyticsDbContext : BaseDbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<LiveStream> Streams { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure minimal User entity - just for relationships
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        // Configure Stream entity with user relationship
        modelBuilder.Entity<LiveStream>()
            .HasOne(s => s.User)
            .WithMany(u => u.Streams)
            .HasForeignKey(s => s.UserId);
            
        // Explicitly ignore all user relationship properties that aren't needed for analytics
        modelBuilder.Entity<User>()
            .Ignore(u => u.FollowingRelationships);
        
        modelBuilder.Entity<User>()
            .Ignore(u => u.FollowedByRelationships);

        // Ignore stream metadata for analytics service
        modelBuilder.Entity<LiveStream>()
            .Ignore(s => s.Metadata);
    }
}
