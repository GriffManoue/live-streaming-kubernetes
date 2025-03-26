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
        
        // Configure User entity relationships
        modelBuilder.Entity<User>()
            .HasMany(u => u.Followers)
            .WithMany(u => u.Following)
            .UsingEntity(j => j.ToTable("UserFollowers"));

        // Configure the User side of one-to-one relationship with LiveStream
        modelBuilder.Entity<User>()
            .HasOne(u => u.Stream)
            .WithOne(s => s.User)
            .HasForeignKey<LiveStream>(s => s.UserId);
    }
}
