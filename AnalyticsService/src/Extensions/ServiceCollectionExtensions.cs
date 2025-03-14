using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Interfaces;
using Shared.Models.Domain;
using System;
using AnalyticsService.Data;

namespace AnalyticsService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnalyticsDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Add AnalyticsDbContext
        services.AddDbContext<AnalyticsDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        // Register AnalyticsDbContext as IDbContext for dependency injection
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<AnalyticsDbContext>());
        
        // Register repositories for Analytics-specific entities
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<LiveStream>, Repository<LiveStream>>();
        
        return services;
    }
}
