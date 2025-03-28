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

        // Configure the table name explicitly to match the database
        modelBuilder.Entity<LiveStream>().ToTable("LiveStreams");
        
        // Configure Stream entity with explicit foreign key
        modelBuilder.Entity<LiveStream>()
            .HasOne(s => s.User)
            .WithOne(u => u.Stream)
            .HasForeignKey<LiveStream>(s => s.UserId)
            .IsRequired();
    }
}
