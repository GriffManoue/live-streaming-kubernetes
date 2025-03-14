using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models.Domain;

namespace StreamService.Data;

public class StreamDbContext : BaseDbContext
{
    public StreamDbContext(DbContextOptions<StreamDbContext> options) : base(options)
    {
    }
    
    public DbSet<LiveStream> Streams { get; set; } = null!;
    public DbSet<StreamMetadata> StreamMetadata { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!; // Need User for relationships
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure User entity - minimal configuration needed for references
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        // Configure Stream entity
        modelBuilder.Entity<LiveStream>()
            .HasOne(s => s.User)
            .WithMany(u => u.Streams)
            .HasForeignKey(s => s.UserId);
        
        // Configure StreamMetadata entity
        modelBuilder.Entity<StreamMetadata>()
            .HasOne(sm => sm.Stream)
            .WithOne(s => s.Metadata)
            .HasForeignKey<StreamMetadata>(sm => sm.StreamId);
            
        // Exclude navigation properties that are not needed for this service
        modelBuilder.Entity<User>()
            .Metadata.FindNavigation(nameof(User.FollowingRelationships))?.SetPropertyAccessMode(PropertyAccessMode.Field);
        
        modelBuilder.Entity<User>()
            .Metadata.FindNavigation(nameof(User.FollowedByRelationships))?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
