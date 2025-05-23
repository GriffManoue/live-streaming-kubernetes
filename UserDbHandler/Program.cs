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
using UserDbHandler.Data;
using UserDbHandler.Services;

// Program.cs for UserDbHandler
// --------------------------------------------------
// Service registration and configuration for UserDbHandler
// --------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add UserDbContext and register as IDbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<UserDbContext>());

// Register open generic repository
builder.Services.AddScoped<IRepository<User>, Repository<User>>();

// Register application services
builder.Services.AddScoped<IUserDbHandlerService, UserDbHandlerService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

// Register IHttpContextAccessor for DI (needed for JwtTokenHandler)
builder.Services.AddHttpContextAccessor();

// Register JwtTokenHandler for DI
builder.Services.AddTransient<JwtTokenHandler>();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserDbHandler API", Version = "v1" });
});

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["Jwt:SecretKey"];
    if (string.IsNullOrEmpty(secretKey))
        throw new InvalidOperationException("JWT SecretKey is not configured.");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ,
        ValidAudience = builder.Configuration["Jwt:Audience"] ,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});

// Add Health Checks and Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (string.IsNullOrEmpty(redisConnectionString))
    throw new InvalidOperationException("Redis connection string is not configured.");
builder.Services.AddHealthChecks()
    .AddDbContextCheck<UserDbContext>(
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready" })
    .AddRedis(
        redisConnectionString,
        "redis",
        HealthStatus.Degraded,
        new[] { "ready" },
        TimeSpan.FromSeconds(10));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configOptions = ConfigurationOptions.Parse(redisConnectionString);
    configOptions.AbortOnConnectFail = false;
    configOptions.ConnectRetry = 5;
    configOptions.ReconnectRetryPolicy = new ExponentialRetry(5000);
    return ConnectionMultiplexer.Connect(configOptions);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserDbHandler API v1"); });


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();