using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Data;
using Shared.Interfaces;
using Shared.Interfaces.Clients;
using Shared.Models.Domain;
using Shared.Services;
using StackExchange.Redis;
using StreamDbHandler.Data;
using StreamDbHandler.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StreamService API", Version = "v1" });
});

// Add Health Checks with more resilient configuration
builder.Services.AddHealthChecks()
    .AddDbContextCheck<StreamDbContext>(
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

// Add StreamDbContext
builder.Services.AddDbContext<StreamDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            5,
            TimeSpan.FromSeconds(30),
            null);
    });
});

// Register StreamDbContext as IDbContext for dependency injection
builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<StreamDbContext>());

// Register repositories for Stream-specific entities
builder.Services.AddScoped<IRepository<LiveStream>, Repository<LiveStream>>();
// Register the JwtTokenHandler
builder.Services.AddTransient<JwtTokenHandler>();

// Add HttpClient for the UserDbHandler
builder.Services.AddHttpClient<IUserDbHandlerClient, UserDbHandlerClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<JwtTokenHandler>(); // Add the handler

builder.Services.AddHttpClient<IStreamDbHandlerClient, StreamDbHandlerClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<JwtTokenHandler>(); // Add the handler

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
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Register IHttpContextAccessor for dependency injection
builder.Services.AddHttpContextAccessor();

// Add application services
builder.Services.AddScoped<IStreamDbHandlerService, StreamDbHandlerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Only use HTTPS redirection in local development, not in containers
    var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!isRunningInContainer) app.UseHttpsRedirection();

    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API v1"); });
}

// IMPORTANT: Place UseCors before Authentication/Authorization
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
