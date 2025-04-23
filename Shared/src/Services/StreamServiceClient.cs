using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models.Domain;
using Shared.Models.Stream;

namespace StreamDbHandler.Services;

public class StreamServiceClient : StreamDbHandler.Services.IStreamServiceClient
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<StreamServiceClient>? _logger;

    public StreamServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<StreamServiceClient>? logger = null)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(15); // Add timeout to prevent long-running requests
        _logger = logger;
        
        // Read from configuration with fallback to the default value
        _baseUrl = configuration["Services:StreamService:BaseUrl"] ?? "http://stream-service/api/";
        _logger?.LogInformation("StreamServiceClient initialized with base URL: {BaseUrl}", _baseUrl);
    }

    public async Task<StreamDto?> GetStreamByIdAsync(Guid streamId)
    {
        try
        {
            var url = $"{_baseUrl}stream/{streamId}";
            _logger?.LogInformation("Getting stream with ID {StreamId} from {Url}", streamId, url);
            return await _httpClient.GetFromJsonAsync<StreamDto>(url);
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to get stream with ID {StreamId}: {Message}", streamId, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error getting stream with ID {StreamId}: {Message}", streamId, ex.Message);
            return null;
        }
    }

    public async Task<StreamDto> CreateStreamAsync(User? user = null)
    {
        try
        {
            // Add userId as a query parameter if user is provided
            string url = user != null 
                ? $"{_baseUrl}stream?userId={user.Id}" 
                : $"{_baseUrl}stream";
            
            _logger?.LogInformation("Creating new stream at {Url}", url);
            
            var response = await _httpClient.PostAsync(url, new StringContent(string.Empty));
            _logger?.LogInformation("Create stream response: {StatusCode}", response.StatusCode);
            
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadFromJsonAsync<StreamDto>();
            
            if (stream == null)
            {
                throw new InvalidOperationException("Stream service returned null response");
            }
            
            _logger?.LogInformation("Stream created successfully with ID {StreamId}", stream.Id);
            return stream;
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Connection error creating stream: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create stream: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error creating stream: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create stream: {ex.Message}", ex);
        }
    }
}