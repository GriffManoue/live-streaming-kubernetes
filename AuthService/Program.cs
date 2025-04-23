using System.Text;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Data;
using Shared.Interfaces;
using Shared.Interfaces.Clients;
using Shared.Services;
using StackExchange.Redis;
using StreamDbHandler.Services;

// Program.cs for AuthService
// --------------------------------------------------
// Service registration and configuration for AuthService
// --------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService API", Version = "v1" });
});

// Register application services
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

// Register IHttpContextAccessor for DI (needed for JwtTokenHandler)
builder.Services.AddHttpContextAccessor();

// Register JwtTokenHandler for DI
builder.Services.AddTransient<JwtTokenHandler>();

// Register HttpClients with JwtTokenHandler for outgoing requests
builder.Services.AddHttpClient<IStreamDbHandlerClient, StreamDbHandlerClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<IUserDbHandlerClient, UserDbHandlerClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<JwtTokenHandler>();

// Add Health Checks and Redis
builder.Services.AddHealthChecks()
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis") ?? "localhost",
        "redis",
        HealthStatus.Degraded,
        new[] { "ready" },
        TimeSpan.FromSeconds(5));
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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "your-256-bit-secret-key-here-at-least-32-chars"))
    };
});

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API v1"); });

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

Console.WriteLine("Starting AuthService...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
app.Run();
