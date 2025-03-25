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
        
         modelBuilder.Entity<LiveStream>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StreamName).IsRequired();
            entity.Property(e => e.StreamDescription).IsRequired();
            entity.Property(e => e.StreamCategory).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.User.Id)
                .IsRequired();
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Email).IsRequired();
        });
        
    }
} 