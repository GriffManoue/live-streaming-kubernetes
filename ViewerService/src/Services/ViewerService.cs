using System;
using Shared.Interfaces;
using Shared.Interfaces.Clients;
using Shared.src.Interfaces.Services;

namespace ViewerService.src.Services;

public class ViewerService : IViewerService
{
    private readonly ILogger<ViewerService> _logger;
    private readonly IStreamDbHandlerClient _streamDbHandlerClient;

    private readonly ICacheService _cacheService;

    public ViewerService(ILogger<ViewerService> logger, IStreamDbHandlerClient streamDbHandlerClient, ICacheService cacheService)
    {
        _logger = logger;
        _streamDbHandlerClient = streamDbHandlerClient;
        _cacheService = cacheService;
    }

    public async Task<int> GetViewerCountAsync(Guid streamId)
    {
        var key = $"stream:viewers:{streamId}";
        _logger.LogInformation("Getting viewer count for stream {StreamId}", streamId);
        return await _cacheService.SetCountAsync(key);
    }

    public async Task JoinViewerAsync(Guid streamId, string viewerId)
    {
        var key = $"stream:viewers:{streamId}";
        _logger.LogInformation("Viewer {ViewerId} joining stream {StreamId}", viewerId, streamId);
        await _cacheService.SetAddAsync(key, viewerId);
        await _cacheService.ExpireAsync(key, TimeSpan.FromHours(6)); // Optional: expire after inactivity

        var stream = await _streamDbHandlerClient.GetStreamByIdAsync(streamId);
        if (stream != null)
        {
            stream.Views++; // Increment view count
            _logger.LogInformation("Incremented view count for stream {StreamId}. New count: {Views}", streamId, stream.Views);
            await _streamDbHandlerClient.UpdateStreamAsync(streamId, stream); // Update the stream in the database
        }
        else
        {
            _logger.LogWarning("Stream {StreamId} not found when joining viewer {ViewerId}", streamId, viewerId);
        }
    }

    public async Task LeaveViewerAsync(Guid streamId, string viewerId)
    {
        var key = $"stream:viewers:{streamId}";
        _logger.LogInformation("Viewer {ViewerId} leaving stream {StreamId}", viewerId, streamId);
        await _cacheService.SetRemoveAsync(key, viewerId);
        var stream = await _streamDbHandlerClient.GetStreamByIdAsync(streamId);
        if (stream != null)
        {
            stream.Views--; // Decrement view count
            _logger.LogInformation("Decremented view count for stream {StreamId}. New count: {Views}", streamId, stream.Views);
            await _streamDbHandlerClient.UpdateStreamAsync(streamId, stream); // Update the stream in the database
        }
        else
        {
            _logger.LogWarning("Stream {StreamId} not found when leaving viewer {ViewerId}", streamId, viewerId);
        }
    }
}
