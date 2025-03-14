using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Interfaces;
using Shared.Models.Domain;
using Shared.Services;
using StackExchange.Redis;
using System;

namespace Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext with retry logic for Kubernetes
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        // Add Redis with connection resilience
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configOptions = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis") ?? "localhost");
            configOptions.AbortOnConnectFail = false; // Don't fail if connection isn't initially available
            configOptions.ConnectRetry = 5;           // Retry connection attempts
            configOptions.ReconnectRetryPolicy = new ExponentialRetry(5000); // Exponential backoff 
            return ConnectionMultiplexer.Connect(configOptions);
        });
        
        // Add Cache Service
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        // Add Repositories
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<LiveStream>, Repository<LiveStream>>();
        services.AddScoped<IRepository<StreamMetadata>, Repository<StreamMetadata>>();
        services.AddScoped<IRepository<UserRelationship>, Repository<UserRelationship>>();
        
        return services;
    }
}
