using Shared.src.Interfaces.Services;
using Shared.Services;
using Shared.Interfaces;
using Shared.Interfaces.Clients;
using StreamDbHandler.Services;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<Shared.src.Interfaces.Services.IViewerService, ViewerService.src.Services.ViewerService>();

// Register HttpClient for StreamDbHandlerClient
builder.Services.AddHttpClient<IStreamDbHandlerClient, StreamDbHandlerClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register Redis multiplexer as singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis") ?? "localhost");
    configOptions.AbortOnConnectFail = false;
    configOptions.ConnectRetry = 5;
    configOptions.ReconnectRetryPolicy = new ExponentialRetry(5000);
    return ConnectionMultiplexer.Connect(configOptions);
});

// Register RedisCacheService as singleton
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddLogging();

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
