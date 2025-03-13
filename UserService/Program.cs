using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Extensions;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost");

// Add shared services (DbContext, Redis, Repositories)
builder.Services.AddSharedServices(builder.Configuration);

// Add application services
builder.Services.AddScoped<Shared.Interfaces.IUserService, UserService.Services.UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
