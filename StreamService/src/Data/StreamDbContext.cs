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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Stream entity
        modelBuilder.Entity<LiveStream>()
            .HasOne(s => s.User)
            .WithOne(u => u.Stream);
    }
}
