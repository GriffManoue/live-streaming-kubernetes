using Microsoft.EntityFrameworkCore;
using Shared.Models.Domain;
using Shared.Data;

namespace UserDbHandler.Data;

public class UserDbContext : BaseDbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        //Ignore Stream entity in UserDbContext
        modelBuilder.Ignore<LiveStream>();

        // Configure many-to-many relationship between users (followers/following)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Followers)
            .WithMany(u => u.Following)
            .UsingEntity(j => j
                .ToTable("UserFollowers") // Use a more descriptive table name
            );
    }
}
