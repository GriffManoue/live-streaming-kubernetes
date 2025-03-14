using System.Threading.Tasks;

namespace DatabaseManagementService.Services;

public interface IMigrationService
{
    /// <summary>
    /// Applies any pending migrations to the database
    /// </summary>
    Task<bool> MigrateAsync();
    
    /// <summary>
    /// Checks if there are any pending migrations that need to be applied
    /// </summary>
    Task<bool> HasPendingMigrationsAsync();
    
    /// <summary>
    /// Gets a list of all available migrations
    /// </summary>
    Task<IEnumerable<string>> GetAvailableMigrationsAsync();
    
    /// <summary>
    /// Gets a list of applied migrations
    /// </summary>
    Task<IEnumerable<string>> GetAppliedMigrationsAsync();
}
