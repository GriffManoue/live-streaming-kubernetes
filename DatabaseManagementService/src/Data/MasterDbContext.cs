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
    // Stream related entities
    public DbSet<LiveStream> LiveStreams { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasMany(u => u.Followers)
                  .WithMany(u => u.Following)
                  .UsingEntity(j => j.ToTable("UserFollowers"));
        });

        // Configure LiveStream entity
        modelBuilder.Entity<LiveStream>(entity =>
        {
            entity.HasKey(ls => ls.Id);
            entity.HasOne(ls => ls.User)
                  .WithOne(u => u.Stream)
                  .HasForeignKey<LiveStream>(ls => ls.UserId);
        });
    }
}