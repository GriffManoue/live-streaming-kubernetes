using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models.Stream;

namespace StreamDbHandler.Services;

public class UserServiceClient : IUserServiceClient
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserServiceClient>? _logger;

    public UserServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<UserServiceClient>? logger = null)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(15); // Add timeout to prevent long-running requests
        _logger = logger;
        _baseUrl = configuration["ServiceUrls:UserService"] ?? "http://user-service/api";
        _logger?.LogInformation("UserServiceClient initialized with base URL: {BaseUrl}", _baseUrl);
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var url = $"{_baseUrl}/user/{userId}";
            _logger?.LogInformation("Getting user with ID {UserId} from {Url}", userId, url);
            return await _httpClient.GetFromJsonAsync<UserDTO>(url);
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to get user with ID {UserId}: {Message}", userId, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error getting user with ID {UserId}: {Message}", userId, ex.Message);
            return null;
        }
    }

    public async Task UpdateUserAsync(Guid userId, UserDTO user)
    {
        try
        {
            var url = $"{_baseUrl}/user/{userId}";
            _logger?.LogInformation("Updating user with ID {UserId} at {Url}", userId, url);
            var response = await _httpClient.PutAsJsonAsync(url, user);
            _logger?.LogInformation("Update user response: {StatusCode}", response.StatusCode);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to update user with ID {UserId}: {Message}", userId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error updating user with ID {UserId}: {Message}", userId, ex.Message);
            throw;
        }
    }
}