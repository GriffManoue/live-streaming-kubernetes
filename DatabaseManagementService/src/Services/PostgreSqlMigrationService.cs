using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseManagementService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseManagementService.Services;

public class PostgreSqlMigrationService : IMigrationService
{
    private readonly MigrationDbContext _dbContext;
    private readonly ILogger<PostgreSqlMigrationService> _logger;

    public PostgreSqlMigrationService(MigrationDbContext dbContext, ILogger<PostgreSqlMigrationService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> MigrateAsync()
    {
        try
        {
            _logger.LogInformation("Starting database migration process");
            
            // Check database connection first
            await _dbContext.Database.CanConnectAsync();
            
            _logger.LogInformation("Applying pending migrations");
            await _dbContext.Database.MigrateAsync();
            
            _logger.LogInformation("Database migration completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while migrating the database");
            return false;
        }
    }

    public async Task<bool> HasPendingMigrationsAsync()
    {
        try
        {
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
            return pendingMigrations.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for pending migrations");
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetAvailableMigrationsAsync()
    {
        try
        {
            return await Task.FromResult(_dbContext.Database.GetMigrations());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available migrations");
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetAppliedMigrationsAsync()
    {
        try
        {
            return await _dbContext.Database.GetAppliedMigrationsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applied migrations");
            throw;
        }
    }
}
