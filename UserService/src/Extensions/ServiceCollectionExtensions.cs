using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Interfaces;
using Shared.Models.Domain;
using System;
using UserService.Data;

namespace UserService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Add UserDbContext
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        // Register UserDbContext as IDbContext for dependency injection
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<UserDbContext>());
        
        // Register repositories for User-specific entities
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<UserRelationship>, Repository<UserRelationship>>();
        
        return services;
    }
}
