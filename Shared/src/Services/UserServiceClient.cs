using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace StreamService.Services;

public class UserServiceClient : IUserServiceClient
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public UserServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["ServiceUrls:UserService"] ?? "http://user-service/api";
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserDTO>($"{_baseUrl}/user/{userId}");
        }
        catch (HttpRequestException)
        {
            // Log the exception and return null
            return null;
        }
    }

    public async Task UpdateUserAsync(Guid userId, UserDTO user)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/user/{userId}", user);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            // Log the exception and rethrow
            throw;
        }
    }
}