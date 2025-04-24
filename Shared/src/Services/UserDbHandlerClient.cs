using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models.Domain;
using Shared.Models.Stream;
using Shared.src.Models.User;

namespace StreamDbHandler.Services;

public class UserDbHandlerClient : IUserDbHandlerClient
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserDbHandlerClient>? _logger;

    public UserDbHandlerClient(HttpClient httpClient, IConfiguration configuration, ILogger<UserDbHandlerClient>? logger = null)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(15); // Add timeout to prevent long-running requests
        _logger = logger;
        _baseUrl = configuration["ServiceUrls:UserDbHandler:BaseUrl"] ?? "http://user-db-handler/api"; //Todo add url to config
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

    public async Task<UserWithFollowersDTO> GetUserByIdWithFollowersAsync(Guid id)
    {
        try
        {
            var url = $"{_baseUrl}/user/includes/{id}";
            _logger?.LogInformation("Getting user with followers (includes) for ID {UserId} from {Url}", id, url);
            var user = await _httpClient.GetFromJsonAsync<UserWithFollowersDTO>(url);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            return user;
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to get user with followers for ID {UserId}: {Message}", id, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error getting user with followers for ID {UserId}: {Message}", id, ex.Message);
            throw;
        }
    }

    public async Task<UserDTO> GetUserByUsernameAsync(string username)
    {
        try
        {
            var url = $"{_baseUrl}/user/username/{username}";
            _logger?.LogInformation("Getting user by username {Username} from {Url}", username, url);
            var user = await _httpClient.GetFromJsonAsync<UserDTO>(url);
            if (user == null)
                throw new KeyNotFoundException($"User with username {username} not found");
            return user;
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to get user by username {Username}: {Message}", username, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error getting user by username {Username}: {Message}", username, ex.Message);
            throw;
        }
    }

    public async Task<UserDTO> UpdateUserAsync(Guid userId, UserDTO user)
    {
        try
        {
            var url = $"{_baseUrl}/user/{userId}";
            _logger?.LogInformation("Updating user with ID {UserId} at {Url}", userId, url);
            var response = await _httpClient.PutAsJsonAsync(url, user);
            _logger?.LogInformation("Update user response: {StatusCode}", response.StatusCode);
            response.EnsureSuccessStatusCode();
            var updatedUser = await response.Content.ReadFromJsonAsync<UserDTO>();
            if (updatedUser == null)
            {
                throw new InvalidOperationException("Failed to deserialize updated user.");
            }
            return updatedUser;
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

    public async Task<UserDTO> CreateUserAsync(UserDTO user)
    {
        try
        {
            var url = $"{_baseUrl}/user";
            _logger?.LogInformation("Creating user at {Url}", url);
            var response = await _httpClient.PostAsJsonAsync(url, user);
            response.EnsureSuccessStatusCode();
            var createdUser = await response.Content.ReadFromJsonAsync<UserDTO>();
            if (createdUser == null)
                throw new InvalidOperationException("Failed to deserialize created user.");
            return createdUser;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error creating user: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<UserDTO?> GetUserByEmailAsync(string email)
    {
        try
        {
            var url = $"{_baseUrl}/user/email/{email}";
            _logger?.LogInformation("Getting user by email {Email} from {Url}", email, url);
            var user = await _httpClient.GetFromJsonAsync<UserDTO>(url);
            return user;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting user by email: {Message}", ex.Message);
            return null;
        }
    }
}