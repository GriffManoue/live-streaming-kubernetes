using Microsoft.EntityFrameworkCore;
using DatabaseManagementService.Data;
using DatabaseManagementService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add migration services - Only MasterDbContext is explicitly configured for migrations
// Other service-specific DbContexts (like UserDbContext) will be migrated by their respective services
builder.Services.AddDbContext<MasterDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMigrationService, PostgreSqlMigrationService>();

// Add swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Database Management Service", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using (var scope = app.Services.CreateScope())
    {
        var migrationService = scope.ServiceProvider.GetRequiredService<DatabaseManagementService.Services.IMigrationService>();
        await migrationService.ApplyMigrationsAsync();
    }


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
