using Microsoft.EntityFrameworkCore;
using Shared.Models.Domain;

namespace UserService.Data;

public class UserDbContext : IDbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Stream)
            .WithOne(s => s.User)
            .HasForeignKey<LiveStream>(s => s.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
