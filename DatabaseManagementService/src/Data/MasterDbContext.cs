using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models.Domain;

namespace DatabaseManagementService.Data;

/// <summary>
/// Master DbContext that includes all tables from all microservices.
/// This context is only used for database migrations and administration tasks.
/// </summary>
public class MasterDbContext : BaseDbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }
    
    // User related entities
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserRelationship> UserRelationships { get; set; } = null!;
    
    // Stream related entities
    public DbSet<LiveStream> LiveStreams { get; set; } = null!;
    public DbSet<StreamMetadata> StreamMetadata { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        // Configure UserRelationship entity
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
            
        // Configure LiveStream entity
        modelBuilder.Entity<LiveStream>()
            .HasOne(ls => ls.User)
            .WithMany(u => u.Streams)
            .HasForeignKey(ls => ls.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure StreamMetadata entity
        modelBuilder.Entity<StreamMetadata>()
            .HasOne(sm => sm.Stream)
            .WithOne(ls => ls.Metadata)
            .HasForeignKey<StreamMetadata>(sm => sm.StreamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 