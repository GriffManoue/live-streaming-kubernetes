using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Interfaces;
using Shared.Services;
using StackExchange.Redis;
using UserService.Data;
using UserService.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "UserService API", Version = "v1" });
});

// Add Health Checks with more resilient configuration
builder.Services.AddHealthChecks()
    .AddDbContextCheck<UserDbContext>(
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready" })
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis") ?? "localhost",
        name: "redis",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready" },
        timeout: TimeSpan.FromSeconds(10));

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

// Add user-specific DB context and repositories
builder.Services.AddUserDbContext(builder.Configuration);

// Add application services
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService v1"));
    
    // Only use HTTPS redirection in local development, not in containers
    var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!isRunningInContainer)
    {
        app.UseHttpsRedirection();
    }
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Apply database migrations on startup
// This calls the DatabaseManagementService to run migrations for UserService
if (app.Environment.IsDevelopment())
{
    try
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(builder.Configuration["Services:DatabaseManagementService"]);
        
        var content = new StringContent("\"UserService\"", System.Text.Encoding.UTF8, "application/json");
        var response = httpClient.PostAsync("/api/migration/migrate", content).GetAwaiter().GetResult();
        
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Database migrations applied successfully");
        }
        else
        {
            Console.WriteLine($"Failed to apply migrations: {response.StatusCode}");
            var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Console.WriteLine($"Error details: {errorContent}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}


app.Run();
