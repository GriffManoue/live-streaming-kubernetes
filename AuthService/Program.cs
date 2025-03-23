using System.Text;
using AuthService.Data;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Data;
using Shared.Interfaces;
using Shared.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add UserDbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register UserDbContext as IDbContext for dependency injection
builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<AuthDbContext>());

// Register the open generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add application services
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService API", Version = "v1" });
});

// Add Health Checks with more resilient configuration
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AuthDbContext>(
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

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost") // Replace with your client's actual origin
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

// Add application services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Only use HTTPS redirection in local development, not in containers
    var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!isRunningInContainer) app.UseHttpsRedirection();

    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API v1"); });
}

// IMPORTANT: Place UseCors before Authentication/Authorization
// Replace the existing UseCors call with a more specific configuration.
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

Console.WriteLine("Starting AuthService...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
app.Run();