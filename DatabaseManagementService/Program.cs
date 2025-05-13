using Microsoft.EntityFrameworkCore;
using DatabaseManagementService.Data;
using DatabaseManagementService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add migration services - Only MasterDbContext is explicitly configured for migrations
builder.Services.AddDbContext<MasterDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMigrationService, PostgreSqlMigrationService>();

// Add swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Database Management Service", Version = "v1" });
});

// Add health checks
builder.Services.AddHealthChecks();

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
    };

    var secretKey = builder.Configuration["Jwt:SecretKey"];
    if (string.IsNullOrEmpty(secretKey))
        throw new InvalidOperationException("JWT SecretKey is not configured.");

    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(secretKey));
});

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DatabaseManagementService API v1");
});

using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();
    await migrationService.ApplyMigrationsAsync();
}


app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
