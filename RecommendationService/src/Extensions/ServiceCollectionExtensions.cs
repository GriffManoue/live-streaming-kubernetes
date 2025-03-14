using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Interfaces;
using Shared.Models.Domain;
using System;
using RecommendationService.Data;

namespace RecommendationService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRecommendationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Add RecommendationDbContext
        services.AddDbContext<RecommendationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        // Register RecommendationDbContext as IDbContext for dependency injection
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<RecommendationDbContext>());
        
        // Register repositories for Recommendation-specific entities
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<LiveStream>, Repository<LiveStream>>();
        services.AddScoped<IRepository<UserRelationship>, Repository<UserRelationship>>();
        
        return services;
    }
}
