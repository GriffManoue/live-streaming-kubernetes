using Microsoft.EntityFrameworkCore;
using DatabaseManagementService.Data;
using DatabaseManagementService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add migration services - Only MasterDbContext is explicitly configured for migrations
builder.Services.AddDbContext<MasterDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMigrationService, PostgreSqlMigrationService>();

builder.Logging.AddConsole();

// Add swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Database Management Service", Version = "v1" });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using (var scope = app.Services.CreateScope())
    {
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();
        await migrationService.ApplyMigrationsAsync();
    }


    // Only use HTTPS redirection in local development, not in containers
    var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!isRunningInContainer)
    {
        app.UseHttpsRedirection();
    }

     app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DatabaseManagementService API v1");
    });
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
