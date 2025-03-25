using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Shared.Models.Stream;

namespace StreamService.Services;

public class StreamServiceClient : IStreamServiceClient
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public StreamServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["ServiceUrls:UserService"] ?? "http://localhost/api/";
    }

    public async Task<StreamDto?> GetStreamByIdAsync(Guid streamId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<StreamDto>($"{_baseUrl}/stream/{streamId}");
        }
        catch (HttpRequestException)
        {
            // Log the exception and return null
            return null;
        }
    }

    public async Task<StreamDto> CreateStreamAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/stream", null);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadFromJsonAsync<StreamDto>();
            return stream ?? throw new InvalidOperationException("Failed to create stream");
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new InvalidOperationException($"Failed to create stream: {ex.Message}", ex);
        }
    }
}