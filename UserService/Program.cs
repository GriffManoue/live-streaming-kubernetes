using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Extensions;
using UserService.Services;
using System;

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
    .AddDbContextCheck<ApplicationDbContext>(
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready" })
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis") ?? "localhost",
        name: "redis",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready" },
        timeout: TimeSpan.FromSeconds(5));

// Add shared services (DbContext, Redis, Repositories)
builder.Services.AddSharedServices(builder.Configuration);

// Add application services
builder.Services.AddScoped<Shared.Interfaces.IUserService, UserService.Services.UserService>();

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

app.Run();
