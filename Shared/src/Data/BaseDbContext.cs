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
        
        // Common configuration like audit fields can go here
        // Service-specific DB contexts will call this base implementation
        // and then add their own specific configurations
    }
}
