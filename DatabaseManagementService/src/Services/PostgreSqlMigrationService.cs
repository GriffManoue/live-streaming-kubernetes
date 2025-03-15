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
    private readonly MigrationDbContext _dbContext;
    private readonly ILogger<PostgreSqlMigrationService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PostgreSqlMigrationService(
        MigrationDbContext dbContext, 
        ILogger<PostgreSqlMigrationService> logger,
        IServiceProvider serviceProvider)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<bool> MigrateAsync(string serviceName = null)
    {
        try
        {
            _logger.LogInformation($"Starting database migration process {(serviceName != null ? $"for {serviceName}" : "")}");
            
            if (serviceName == null)
            {
                // Default behavior - migrate using the main context
                return await MigrateDefaultContextAsync();
            }
            
            // Handle migrations for specific services
            return await MigrateServiceContextAsync(serviceName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while migrating the database {(serviceName != null ? $"for {serviceName}" : "")}");
            return false;
        }
    }

    private async Task<bool> MigrateDefaultContextAsync()
    {
        try
        {
            // Check database connection first
            await _dbContext.Database.CanConnectAsync();
            
            _logger.LogInformation("Applying pending migrations for default context");
            await _dbContext.Database.MigrateAsync();
            
            _logger.LogInformation("Database migration completed successfully for default context");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while migrating the default database context");
            return false;
        }
    }

    private async Task<bool> MigrateServiceContextAsync(string serviceName)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            DbContext serviceDbContext = GetDbContextForService(scope.ServiceProvider, serviceName);

            if (serviceDbContext == null)
            {
                _logger.LogError($"Failed to resolve DbContext for service: {serviceName}");
                return false;
            }

            // Check database connection first
            await serviceDbContext.Database.CanConnectAsync();
            
            _logger.LogInformation($"Applying pending migrations for {serviceName}");
            await serviceDbContext.Database.MigrateAsync();
            
            _logger.LogInformation($"Database migration completed successfully for {serviceName}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while migrating the database for {serviceName}");
            return false;
        }
    }

    private DbContext GetDbContextForService(IServiceProvider serviceProvider, string serviceName)
    {
        // Map service name to expected DbContext type name
        string dbContextTypeName = serviceName switch
        {
            "UserService" => "UserService.Data.UserDbContext",
            // Add more service mappings as needed
            _ => throw new ArgumentException($"Unknown service name: {serviceName}")
        };

        // Try to find the DbContext type by name
        Type dbContextType = null;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            dbContextType = assembly.GetTypes()
                .FirstOrDefault(t => t.FullName == dbContextTypeName || t.Name == dbContextTypeName);
            
            if (dbContextType != null)
                break;
        }

        if (dbContextType == null)
        {
            _logger.LogError($"Could not find DbContext type for service: {serviceName}");
            return null;
        }

        // Try to get the DbContext instance from the service provider
        try
        {
            return serviceProvider.GetService(dbContextType) as DbContext;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get instance of DbContext for service: {serviceName}");
            return null;
        }
    }

    public async Task<bool> HasPendingMigrationsAsync(string serviceName = null)
    {
        try
        {
            if (serviceName == null)
            {
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                return pendingMigrations.Any();
            }
            
            // Check for pending migrations in the specified service
            using var scope = _serviceProvider.CreateScope();
            DbContext serviceDbContext = GetDbContextForService(scope.ServiceProvider, serviceName);
            
            if (serviceDbContext == null)
            {
                _logger.LogError($"Failed to resolve DbContext for service: {serviceName}");
                return false;
            }
            
            var servicePendingMigrations = await serviceDbContext.Database.GetPendingMigrationsAsync();
            return servicePendingMigrations.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking for pending migrations {(serviceName != null ? $"for {serviceName}" : "")}");
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetAvailableMigrationsAsync(string serviceName = null)
    {
        try
        {
            if (serviceName == null)
            {
                return await Task.FromResult(_dbContext.Database.GetMigrations());
            }
            
            // Get available migrations for the specified service
            using var scope = _serviceProvider.CreateScope();
            DbContext serviceDbContext = GetDbContextForService(scope.ServiceProvider, serviceName);
            
            if (serviceDbContext == null)
            {
                _logger.LogError($"Failed to resolve DbContext for service: {serviceName}");
                return Enumerable.Empty<string>();
            }
            
            return await Task.FromResult(serviceDbContext.Database.GetMigrations());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting available migrations {(serviceName != null ? $"for {serviceName}" : "")}");
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetAppliedMigrationsAsync(string serviceName = null)
    {
        try
        {
            if (serviceName == null)
            {
                return await _dbContext.Database.GetAppliedMigrationsAsync();
            }
            
            // Get applied migrations for the specified service
            using var scope = _serviceProvider.CreateScope();
            DbContext serviceDbContext = GetDbContextForService(scope.ServiceProvider, serviceName);
            
            if (serviceDbContext == null)
            {
                _logger.LogError($"Failed to resolve DbContext for service: {serviceName}");
                return Enumerable.Empty<string>();
            }
            
            return await serviceDbContext.Database.GetAppliedMigrationsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting applied migrations {(serviceName != null ? $"for {serviceName}" : "")}");
            throw;
        }
    }
}
