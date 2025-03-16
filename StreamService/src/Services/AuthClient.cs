using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Shared.Models.Auth;

namespace StreamService.Services;

public class AuthClient : IAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly string _authServiceBaseUrl;

    public AuthClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _authServiceBaseUrl = configuration["Services:AuthService"] ?? "http://auth-service";
    }

    public async Task<AuthResult> ValidateTokenAsync(string token)
    {
        try
        {
            var request = new { Token = token };
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"{_authServiceBaseUrl}/api/Auth/validate",
                content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResult>() ??
                       new AuthResult { Success = false, Error = "Failed to deserialize response" };
            }

            return new AuthResult
            {
                Success = false,
                Error = $"Failed to validate token: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Error = $"Error validating token: {ex.Message}"
            };
        }
    }
} 