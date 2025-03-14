using Microsoft.EntityFrameworkCore;

namespace DatabaseManagementService.Data;

public class MigrationDbContext : DbContext
{
    public MigrationDbContext(DbContextOptions<MigrationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Migration specific schema configurations can be added here if needed
    }
    
    // The MigrationDbContext is only used for managing migrations and doesn't need to define entities
    // It will use EF Core's migration infrastructure to apply migrations to the database
}
