using System.Threading.Tasks;
using System.Collections.Generic;

namespace DatabaseManagementService.Services;

public interface IMigrationService
{
    /// <summary>
    /// Applies any pending migrations to the database
    /// </summary>
    /// <param name="serviceName">Optional service name to migrate specific service database</param>
    Task<bool> MigrateAsync(string serviceName = null);
    
    /// <summary>
    /// Checks if there are any pending migrations that need to be applied
    /// </summary>
    /// <param name="serviceName">Optional service name to check specific service database</param>
    Task<bool> HasPendingMigrationsAsync(string serviceName = null);
    
    /// <summary>
    /// Gets a list of all available migrations
    /// </summary>
    /// <param name="serviceName">Optional service name to get migrations for specific service</param>
    Task<IEnumerable<string>> GetAvailableMigrationsAsync(string serviceName = null);
    
    /// <summary>
    /// Gets a list of applied migrations
    /// </summary>
    /// <param name="serviceName">Optional service name to get applied migrations for specific service</param>
    Task<IEnumerable<string>> GetAppliedMigrationsAsync(string serviceName = null);
}
