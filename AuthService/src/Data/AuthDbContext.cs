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

                // Configure the table name explicitly to match the database
        modelBuilder.Entity<LiveStream>().ToTable("LiveStreams");
        
        // Configure User entity - Only include what's needed for authentication
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            
        // Configure the User side of one-to-one relationship with LiveStream
        modelBuilder.Entity<User>()
            .HasOne(u => u.Stream)
            .WithOne(s => s.User)
            .HasForeignKey<LiveStream>(s => s.UserId);
    }
}
