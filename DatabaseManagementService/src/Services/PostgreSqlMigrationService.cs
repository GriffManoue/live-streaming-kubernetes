using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseManagementService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseManagementService.Services;

public class PostgreSqlMigrationService : IMigrationService
{
    private readonly MasterDbContext _masterDbContext;
    private readonly ILogger<PostgreSqlMigrationService> _logger;

    public PostgreSqlMigrationService(
        MasterDbContext masterDbContext,
        ILogger<PostgreSqlMigrationService> logger
        )
    {
        _masterDbContext = masterDbContext ?? throw new ArgumentNullException(nameof(masterDbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HasPendingMigrationsAsync()
    {
        int maxRetries = 5;
        int retryDelay = 5000; // 5 seconds
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation("Checking for pending migrations (attempt {Attempt}/{MaxAttempts})", attempt, maxRetries);
                
                if (!await _masterDbContext.Database.CanConnectAsync())
                {
                    _logger.LogWarning("Cannot connect to database (attempt {Attempt}/{MaxAttempts})", attempt, maxRetries);
                    
                    if (attempt < maxRetries)
                    {
                        _logger.LogInformation("Retrying in {Delay}ms...", retryDelay);
                        await Task.Delay(retryDelay);
                        continue;
                    }
                    return false;
                }
                
                var pendingMigrations = await _masterDbContext.Database.GetPendingMigrationsAsync();
                var hasPending = pendingMigrations.Any();
                _logger.LogInformation("Pending migrations: {Count}", pendingMigrations.Count());
                return hasPending;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for pending migrations (attempt {Attempt}/{MaxAttempts})", attempt, maxRetries);
                
                if (attempt < maxRetries)
                {
                    _logger.LogInformation("Retrying in {Delay}ms...", retryDelay);
                    await Task.Delay(retryDelay);
                }
                else
                {
                    return false;
                }
            }
        }
        
        return false;
    }

    public async Task<IEnumerable<string>> ApplyMigrationsAsync()
    {
        try
        {
            _logger.LogInformation("Applying migrations for MasterDbContext only");
            
            // Get list of pending migrations before applying them
            var pendingMigrations = await _masterDbContext.Database.GetPendingMigrationsAsync();
            var pendingList = pendingMigrations.ToList();
            
            if (pendingList.Any())
            {
                _logger.LogInformation("Found {Count} pending migrations to apply", pendingList.Count);
                await _masterDbContext.Database.MigrateAsync();
                _logger.LogInformation("Successfully applied migrations: {@Migrations}", pendingList);
            }
            else
            {
                _logger.LogInformation("No pending migrations to apply");
            }
            
            return pendingList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying migrations");
            // Return empty list instead of crashing
            return new List<string>();
        }
    }
}
