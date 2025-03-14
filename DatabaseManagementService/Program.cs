using DatabaseManagementService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add migration services
builder.Services.AddMigrationServices(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

// Optional: Run migrations on startup in development environment
// This can be useful during development, but for production
// you might want to run migrations through the API endpoint instead
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var migrationService = scope.ServiceProvider.GetRequiredService<DatabaseManagementService.Services.IMigrationService>();
        await migrationService.MigrateAsync();
    }

    // Only use HTTPS redirection in local development, not in containers
    var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    if (!isRunningInContainer)
    {
        app.UseHttpsRedirection();
    }
}

app.Run();
