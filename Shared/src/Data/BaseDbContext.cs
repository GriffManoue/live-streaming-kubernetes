using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;

namespace Shared.Data;

public abstract class BaseDbContext : DbContext, IDbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
