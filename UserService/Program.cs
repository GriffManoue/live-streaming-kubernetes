using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Shared.Data;
using Shared.Interfaces;
using Shared.Models.Domain;
using Shared.Services;
using StackExchange.Redis;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add UserDbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register UserDbContext as IDbContext for dependency injection
builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<UserDbContext>());

// Register the open generic repository
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IRepository<UserRelationship>, Repository<UserRelationship>>();

// Add application services
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService API", Version = "v1" });
});

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

// Add Health Checks with more resilient configuration
builder.Services.AddHealthChecks()
    .AddDbContextCheck<UserDbContext>(
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready" })
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis") ?? "localhost",
        "redis",
        HealthStatus.Degraded,
        new[] { "ready" },
        TimeSpan.FromSeconds(10));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        new CorsPolicyBuilder()
            .WithOrigins(" http://client-service.default.svc.cluster.local")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .Build());
});

// Add Redis caching service (still shared)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis") ?? "localhost");
    configOptions.AbortOnConnectFail = false;
    configOptions.ConnectRetry = 5;
    configOptions.ReconnectRetryPolicy = new ExponentialRetry(5000);
    return ConnectionMultiplexer.Connect(configOptions);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Only use HTTPS redirection in local development, not in containers
    var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!isRunningInContainer) app.UseHttpsRedirection();

    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API v1"); });
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();