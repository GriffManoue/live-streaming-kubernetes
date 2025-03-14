using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Interfaces;
using Shared.Models.Domain;
using System;
using StreamService.Data;

namespace StreamService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStreamDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Add StreamDbContext
        services.AddDbContext<StreamDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        // Register StreamDbContext as IDbContext for dependency injection
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<StreamDbContext>());
        
        // Register repositories for Stream-specific entities
        services.AddScoped<IRepository<LiveStream>, Repository<LiveStream>>();
        services.AddScoped<IRepository<StreamMetadata>, Repository<StreamMetadata>>();
        services.AddScoped<IRepository<User>, Repository<User>>();
        
        return services;
    }
}
