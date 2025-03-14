using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models.Domain;

namespace RecommendationService.Data;

public class RecommendationDbContext : BaseDbContext
{
    public RecommendationDbContext(DbContextOptions<RecommendationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<LiveStream> Streams { get; set; } = null!;
    public DbSet<UserRelationship> UserRelationships { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        // Configure Stream entity
        modelBuilder.Entity<LiveStream>()
            .HasOne(s => s.User)
            .WithMany(u => u.Streams)
            .HasForeignKey(s => s.UserId);
        
        // Configure UserRelationship entity - needed for recommendation algorithms
        modelBuilder.Entity<UserRelationship>()
            .HasOne(ur => ur.Follower)
            .WithMany(u => u.FollowingRelationships)
            .HasForeignKey(ur => ur.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<UserRelationship>()
            .HasOne(ur => ur.Following)
            .WithMany(u => u.FollowedByRelationships)
            .HasForeignKey(ur => ur.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<UserRelationship>()
            .HasIndex(ur => new { ur.FollowerId, ur.FollowingId })
            .IsUnique();
            
        // Exclude navigation properties that are not needed for this service
        modelBuilder.Entity<LiveStream>()
            .Metadata.FindNavigation(nameof(LiveStream.Metadata))?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
