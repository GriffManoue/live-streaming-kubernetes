using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models.Domain;

namespace UserService.Data;

public class UserDbContext : BaseDbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
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
            
        // Exclude navigation properties that are not needed for this service
        modelBuilder.Entity<User>()
            .Metadata.FindNavigation(nameof(User.Streams))?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
