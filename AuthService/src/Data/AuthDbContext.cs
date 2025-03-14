using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models.Domain;

namespace AuthService.Data;

public class AuthDbContext : BaseDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure User entity - Only include what's needed for authentication
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            
        // Exclude navigation properties that are not needed for this service
        modelBuilder.Entity<User>()
            .Metadata.FindNavigation(nameof(User.Streams))?.SetPropertyAccessMode(PropertyAccessMode.Field);
            
        modelBuilder.Entity<User>()
            .Metadata.FindNavigation(nameof(User.FollowingRelationships))?.SetPropertyAccessMode(PropertyAccessMode.Field);
        
        modelBuilder.Entity<User>()
            .Metadata.FindNavigation(nameof(User.FollowedByRelationships))?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
