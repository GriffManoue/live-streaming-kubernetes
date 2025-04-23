using Microsoft.OpenApi.Models;
using Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add API explorer and Swagger for documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StreamService API", Version = "v1" });
});

// Add HttpContextAccessor for user context support
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();

// Register your main service for dependency injection (implement these later)
 builder.Services.AddScoped<IStreamService, StreamService.src.Services.StreamService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
