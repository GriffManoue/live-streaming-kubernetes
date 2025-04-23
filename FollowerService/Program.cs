using System.Text;
using FollowerService.src.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.src.Interfaces.Services;
using StreamDbHandler.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add API explorer and Swagger for documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FollowerService API", Version = "v1" });
});

// Make sure to use the correct namespace and class name for your implementation
builder.Services.AddScoped<IFollowerService, FollowerService.src.Services.FollowerService>();

// Register IHttpContextAccessor for JwtTokenHandler
builder.Services.AddHttpContextAccessor();

// Register JwtTokenHandler for DI
builder.Services.AddTransient<Shared.Services.JwtTokenHandler>();

// Register HttpClient for IUserDbHandlerClient with JwtTokenHandler
builder.Services.AddHttpClient<IUserDbHandlerClient, UserDbHandlerClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    // Optionally set BaseAddress here if needed
}).AddHttpMessageHandler<Shared.Services.JwtTokenHandler>();

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "streaming-platform",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "streaming-users",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ??
                                       "your-256-bit-secret-key-here-at-least-32-chars"))
        };
    });

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "FollowerService API v1"); });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Add Authentication middleware
app.UseAuthorization();

app.MapControllers();

app.Run();
