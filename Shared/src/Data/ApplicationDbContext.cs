using Microsoft.EntityFrameworkCore;
using Shared.Models.Domain;

namespace Shared.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<LiveStream> Streams { get; set; } = null!;
    public DbSet<StreamMetadata> StreamMetadata { get; set; } = null!;
    public DbSet<UserRelationship> UserRelationships { get; set; } = null!;
    
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
    }
}
