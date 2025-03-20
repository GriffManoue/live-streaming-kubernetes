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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
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
    }
}
