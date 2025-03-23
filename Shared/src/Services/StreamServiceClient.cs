using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Shared.Interfaces;
using Shared.Models.Stream;
using StreamService.Services;

namespace AuthService.Services;

public class StreamServiceClient : IStreamServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly string _streamServiceUrl;

    public StreamServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _streamServiceUrl = configuration["StreamServiceUrl"] ?? throw new ArgumentNullException("StreamServiceUrl");
        _httpClient.BaseAddress = new Uri(_streamServiceUrl);
    }

    public async Task<StreamDto?> GetStreamByIdAsync(Guid streamId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/stream/{streamId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<StreamDto>();
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting stream by id: {ex.Message}");
            return null;
        }
    }
}