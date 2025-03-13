using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Interfaces;
using Shared.Models.Domain;
using Shared.Services;
using StackExchange.Redis;

namespace Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        // Add Redis
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost"));
        
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
