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
        // Register main migration DbContext
        services.AddDbContext<MigrationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Register external service DbContexts for migration
        RegisterExternalDbContexts(services, configuration);

        // Register migration service
        services.AddScoped<IMigrationService, PostgreSqlMigrationService>();

        return services;
    }

    private static void RegisterExternalDbContexts(IServiceCollection services, IConfiguration configuration)
    {
        // Register UserService DbContext using reflection to avoid direct reference dependency
        RegisterDbContextByName(services, configuration, "UserService.Data.UserDbContext");
        
        // Add registrations for other services as needed
    }

    private static void RegisterDbContextByName(IServiceCollection services, IConfiguration configuration, string contextTypeName)
    {
        try
        {
            // Find the DbContext type by name
            Type dbContextType = Type.GetType(contextTypeName) ??
                                 AppDomain.CurrentDomain.GetAssemblies()
                                     .SelectMany(a => a.GetTypes())
                                     .FirstOrDefault(t => t.FullName == contextTypeName);

            if (dbContextType == null)
            {
                // Log warning but don't fail - the service might not be referenced in this project
                Console.WriteLine($"Warning: DbContext type '{contextTypeName}' not found. Migration service will not be able to migrate this context.");
                return;
            }

            // Get the generic AddDbContext method
            var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods()
                .FirstOrDefault(m => m.Name == nameof(EntityFrameworkServiceCollectionExtensions.AddDbContext) && 
                                    m.IsGenericMethod);

            if (addDbContextMethod == null)
            {
                Console.WriteLine("Warning: AddDbContext method not found.");
                return;
            }

            // Create the generic version with our specific DbContext type
            var genericMethod = addDbContextMethod.MakeGenericMethod(dbContextType);

            // Call the method with the appropriate parameters
            genericMethod.Invoke(null, new object[] 
            { 
                services, 
                new Action<DbContextOptionsBuilder>(options => 
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))), 
                ServiceLifetime.Scoped, 
                ServiceLifetime.Scoped 
            });

            Console.WriteLine($"Successfully registered DbContext type '{contextTypeName}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error registering DbContext '{contextTypeName}': {ex.Message}");
        }
    }
}
