using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Shared.Interfaces;
using Shared.Services;
using StackExchange.Redis;
using RecommendationService.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Domain;
using Shared.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RecommendationService API", Version = "v1" });
});

builder.Services.AddOpenApi();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<RecommendationDbContext>(
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready" })
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis") ?? "localhost",
        "redis",
        HealthStatus.Degraded,
        new[] { "ready" },
        TimeSpan.FromSeconds(5));

// Add Redis caching service (shared)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis") ?? "localhost");
    configOptions.AbortOnConnectFail = false;
    configOptions.ConnectRetry = 5;
    configOptions.ReconnectRetryPolicy = new ExponentialRetry(5000);
    return ConnectionMultiplexer.Connect(configOptions);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Add recommendation-specific DB context and repositories
builder.Services.AddDbContext<RecommendationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
    {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });
        
        // Register RecommendationDbContext as IDbContext for dependency injection
builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<RecommendationDbContext>());
        
        // Register repositories for Recommendation-specific entities
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IRepository<LiveStream>, Repository<LiveStream>>();
builder.Services.AddScoped<IRepository<UserRelationship>, Repository<UserRelationship>>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "streaming-platform",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "streaming-users",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ??
                                       "your-256-bit-secret-key-here-at-least-32-chars"))
        };
    });

// Add application services
builder.Services.AddScoped<IRecommendationService, RecommendationService.Services.RecommendationService>();
// The AnalyticsService will be provided by the AnalyticsService microservice

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Only use HTTPS redirection in local development, not in containers
    var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!isRunningInContainer) app.UseHttpsRedirection();

    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecommendationService API v1"); });
}

// IMPORTANT: Place UseCors before Authentication/Authorization
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
