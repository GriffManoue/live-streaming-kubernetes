using Microsoft.OpenApi.Models;
using Shared.Interfaces;
using Shared.Interfaces.Clients;
using Shared.Services;
using StreamDbHandler.Services;
using StackExchange.Redis;

// Program.cs for StreamService
// --------------------------------------------------
// Service registration and configuration for StreamService
// --------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StreamService API", Version = "v1" });
});

// Register Redis multiplexer and cache service
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis") ?? "localhost");
    configOptions.AbortOnConnectFail = false;
    configOptions.ConnectRetry = 5;
    configOptions.ReconnectRetryPolicy = new ExponentialRetry(5000);
    return ConnectionMultiplexer.Connect(configOptions);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Register IHttpContextAccessor for DI (needed for JwtTokenHandler)
builder.Services.AddHttpContextAccessor();

// Register JwtTokenHandler for DI
builder.Services.AddTransient<JwtTokenHandler>();

// Register HttpClients with JwtTokenHandler for outgoing requests
builder.Services.AddHttpClient<IUserDbHandlerClient, UserDbHandlerClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:UserService:BaseUrl"] ?? "http://user-db-handler/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<JwtTokenHandler>();

builder.Services.AddHttpClient<IStreamDbHandlerClient, StreamDbHandlerClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:StreamService:BaseUrl"] ?? "http://stream-db-handler/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddLogging();
builder.Services.AddHealthChecks();

// Register application services
builder.Services.AddScoped<IStreamService, StreamService.src.Services.StreamService>();

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StreamService API V1");
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
