using System.Threading.Tasks;
using System.Collections.Generic;

namespace DatabaseManagementService.Services;

public interface IMigrationService
{
    /// <summary>
    /// Checks if the database has any pending migrations that need to be applied
    /// </summary>
    /// <returns>True if there are pending migrations, false otherwise</returns>
    Task<bool> HasPendingMigrationsAsync();

    /// <summary>
    /// Applies all pending migrations to the database
    /// </summary>
    /// <returns>A collection of applied migration names, or empty if no migrations were applied</returns>
    Task<IEnumerable<string>> ApplyMigrationsAsync();
}
