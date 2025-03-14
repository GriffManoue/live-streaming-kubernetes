using System;
using DatabaseManagementService.Data;
using DatabaseManagementService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseManagementService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMigrationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add MigrationDbContext
        services.AddDbContext<MigrationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        // Register PostgreSQL migration service
        services.AddScoped<IMigrationService, PostgreSqlMigrationService>();
        
        return services;
    }
}
