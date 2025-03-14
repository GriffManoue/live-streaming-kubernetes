using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Interfaces;
using Shared.Models.Domain;
using System;
using AuthService.Data;

namespace AuthService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Add AuthDbContext
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        // Register AuthDbContext as IDbContext for dependency injection
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<AuthDbContext>());
        
        // Register repositories for Auth-specific entities
        services.AddScoped<IRepository<User>, Repository<User>>();
        
        return services;
    }
}
