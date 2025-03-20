using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Shared.Models.User;

namespace StreamService.Services;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public UserServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["ServiceUrls:UserService"] ?? "http://user-service/api";
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserDto>($"{_baseUrl}/user/{userId}");
        }
        catch (HttpRequestException)
        {
            // Log the exception and return null
            return null;
        }
    }
}
