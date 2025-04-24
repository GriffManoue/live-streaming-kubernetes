using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Models.Domain;
using Shared.Models.Stream;
using Shared.Interfaces.Clients;

namespace StreamDbHandler.Services;

public class StreamDbHandlerClient : IStreamDbHandlerClient
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
    private readonly ILogger<StreamDbHandlerClient>? _logger;

    public StreamDbHandlerClient(HttpClient httpClient, IConfiguration configuration, ILogger<StreamDbHandlerClient>? logger = null)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(15); // Add timeout to prevent long-running requests
        _logger = logger;
        
        // Read from configuration with fallback to the default value
        _baseUrl = configuration["Services:StreamDbHandler:BaseUrl"] ?? "http://stream-db-handler/api/"; //Todo: Add url to configuration 
        _logger?.LogInformation("StreamDbHandlerClient initialized with base URL: {BaseUrl}", _baseUrl);
    }

    public async Task<StreamDto?> GetStreamByIdAsync(Guid streamId)
    {
        try
        {
            var url = $"{_baseUrl}streamdbhandler/{streamId}";
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

    public async Task<StreamDto> CreateStreamAsync(Guid userId)
    {
        try
        {
            var url = $"{_baseUrl}streamdbhandler?userId={userId}";
            
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

    public async Task<IEnumerable<StreamDto>> GetActiveStreamsAsync()
    {
        try
        {
            var url = $"{_baseUrl}streamdbhandler/active";
            _logger?.LogInformation("Getting active streams from {Url}", url);
            var streams = await _httpClient.GetFromJsonAsync<IEnumerable<StreamDto>>(url);
            return streams ?? Enumerable.Empty<StreamDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to get active streams: {Message}", ex.Message);
            return Enumerable.Empty<StreamDto>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error getting active streams: {Message}", ex.Message);
            return Enumerable.Empty<StreamDto>();
        }
    }

    public async Task<StreamDto?> GetStreamByUserIdAsync(Guid userId)
    {
        try
        {
            var url = $"{_baseUrl}streamdbhandler/user/{userId}";
            _logger?.LogInformation("Getting stream for user ID {UserId} from {Url}", userId, url);
            return await _httpClient.GetFromJsonAsync<StreamDto>(url);
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to get stream for user ID {UserId}: {Message}", userId, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error getting stream for user ID {UserId}: {Message}", userId, ex.Message);
            return null;
        }
    }

    public async Task<StreamDto?> UpdateStreamAsync(Guid id, StreamDto streamDto)
    {
        try
        {
            var url = $"{_baseUrl}streamdbhandler/{id}";
            _logger?.LogInformation("Updating stream with ID {StreamId} at {Url}", id, url);
            var response = await _httpClient.PutAsJsonAsync(url, streamDto);
            response.EnsureSuccessStatusCode();
            var updatedStream = await response.Content.ReadFromJsonAsync<StreamDto>();
            if (updatedStream == null)
            {
                throw new InvalidOperationException("Stream service returned null response after update");
            }
            _logger?.LogInformation("Stream with ID {StreamId} updated successfully", id);
            return updatedStream;
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to update stream with ID {StreamId}: {Message}", id, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error updating stream with ID {StreamId}: {Message}", id, ex.Message);
            return null;
        }
    }

    public async Task<IEnumerable<StreamDto>> GetAllStreamsAsync()
    {
        try
        {
            var url = $"{_baseUrl}streamdbhandler/all";
            _logger?.LogInformation("Getting all streams from {Url}", url);
            var streams = await _httpClient.GetFromJsonAsync<IEnumerable<StreamDto>>(url);
            return streams ?? Enumerable.Empty<StreamDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger?.LogError(ex, "Failed to get all streams: {Message}", ex.Message);
            return Enumerable.Empty<StreamDto>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error getting all streams: {Message}", ex.Message);
            return Enumerable.Empty<StreamDto>();
        }
    }
}