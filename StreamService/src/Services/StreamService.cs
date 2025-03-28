using Shared.Interfaces;
using Shared.models.Enums;
using Shared.Models.Domain;
using Shared.Models.Stream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamService.Services;

public class StreamService : IStreamService
{
    private readonly IRepository<LiveStream> _streamRepository;
    private readonly ICacheService _cacheService;
    private readonly IUserServiceClient _userServiceClient;
    private readonly IUserContext _userContext;

    public StreamService(
        IRepository<LiveStream> streamRepository,
        ICacheService cacheService,
        IUserServiceClient userServiceClient,
        IUserContext userContext)
    {
        _streamRepository = streamRepository;
        _cacheService = cacheService;
        _userServiceClient = userServiceClient;
        _userContext = userContext;
    }

    public async Task<StreamDto> GetStreamByIdAsync(Guid id)
    {
        var stream = await _streamRepository.GetByIdAsync(id);
        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with ID {id} not found");
        }

        var user = await _userServiceClient.GetUserByIdAsync(stream.UserId);

        return MapToDto(stream, user);
    }

    public async Task<IEnumerable<StreamDto>> GetActiveStreamsAsync()
    {
        // Try to get from cache first
        var cachedStreams = await _cacheService.GetAsync<List<StreamDto>>("active_streams");
        if (cachedStreams != null)
        {
            return cachedStreams;
        }

        // If not in cache, get from database
        var streams = await _streamRepository.GetAllAsync();

        var result = new List<StreamDto>();
        foreach (var stream in streams)
        {
            if (stream.UserId != Guid.Empty)
            {
                var user = await _userServiceClient.GetUserByIdAsync(stream.UserId);
                result.Add(MapToDto(stream, user));
            }
        }

        // Cache the result
        await _cacheService.SetAsync("active_streams", result, TimeSpan.FromMinutes(1));

        return result;
    }

    public async Task<StreamDto> GetStreamByUserIdAsync(Guid userId)
    {
        var user = await _userServiceClient.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        var streams = await _streamRepository.GetAllAsync();
        
        // Find stream where the UserId property matches instead of accessing User.Id which might be null
        var stream = streams.FirstOrDefault(s => s.UserId == userId);

        if (stream == null)
        {
            // Return null instead of throwing an exception for not finding a stream
            return null;
        }

        return MapToDto(stream, user);
    }

    public async Task<StreamDto> CreateStreamAsync(Guid? specifiedUserId = null)
    {
        if (specifiedUserId != null)
        {
            var user = await _userServiceClient.GetUserByIdAsync(specifiedUserId.Value);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {specifiedUserId} not found");
            }

            var streams = await _streamRepository.GetAllAsync();
            if (streams.Any(s => s.UserId == specifiedUserId))
            {
                throw new InvalidOperationException("User already has an active stream");
            }

            var stream = new LiveStream
            {
                Id = Guid.NewGuid(),
                UserId = specifiedUserId.Value, // Set the required UserId property
                StreamName = "New Stream",
                StreamDescription = "Stream Description",
                StreamCategory = StreamCategory.Gaming,
                ThumbnailUrl = "",
                StreamUrl = "",
                Views = 0
            };

            await _streamRepository.AddAsync(stream);
            return MapToDto(stream, user);
        }

        throw new InvalidOperationException("User ID is required to create a stream");
    }

    public async Task<StreamDto> UpdateStreamAsync(Guid id, StreamDto streamDto)
    {
        var stream = await _streamRepository.GetByIdAsync(id);
        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with ID {id} not found");
        }

        // Update stream properties
        if (!string.IsNullOrEmpty(streamDto.StreamName))
        {
            stream.StreamName = streamDto.StreamName;
        }

        if (!string.IsNullOrEmpty(streamDto.StreamDescription))
        {
            stream.StreamDescription = streamDto.StreamDescription;
        }

        stream.StreamCategory = streamDto.StreamCategory;

        if (!string.IsNullOrEmpty(streamDto.ThumbnailUrl))
        {
            stream.ThumbnailUrl = streamDto.ThumbnailUrl;
        }

        if (!string.IsNullOrEmpty(streamDto.StreamUrl))
        {
            stream.StreamUrl = streamDto.StreamUrl;
        }

        stream.Views = streamDto.Views;

        await _streamRepository.UpdateAsync(stream);

        // Invalidate cache
        await _cacheService.RemoveAsync("active_streams");

        var user = await _userServiceClient.GetUserByIdAsync(stream.UserId);

        return MapToDto(stream, user);
    }

    public async Task<string> GenerateStremKeyAsync(Guid id)
    {
        var stream = await _streamRepository.GetByIdAsync(id);
        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with ID {id} not found");
        }

        // Generate a new stream key
        var streamKey = Guid.NewGuid().ToString();

        // Update the stream with the new key
        stream.StreamUrl = streamKey;

        await _streamRepository.UpdateAsync(stream);

        return streamKey;
    }

    public async Task<string> GenerateStreamKeyAsync(Guid id)
    {
        var stream = await _streamRepository.GetByIdAsync(id);
        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with ID {id} not found");
        }

        // Generate a new stream key
        var streamKey = Guid.NewGuid().ToString();

        // Store the raw stream key directly
        // We'll use this exact key for RTMP streaming
        stream.StreamKey = streamKey;
        
        // Update the stream URL for HLS playback
        stream.StreamUrl = "http://localhost:8080/hls/" + streamKey + ".m3u8";

        await _streamRepository.UpdateAsync(stream);

        return streamKey;
    }

    public async Task StartStreamAsync(string streamKey)
    {
        // Validate the stream key
        var streams = await _streamRepository.GetAllAsync();

        // Find the stream with the given key by matching against StreamKey property
        var stream = streams.FirstOrDefault(s => s.StreamKey == streamKey);

        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with key {streamKey} not found");
        }

        // Mark the stream as active
        // You might want to update additional properties to indicate the stream is live

        await _cacheService.RemoveAsync("active_streams");
    }

    public async Task EndStreamAsync(string streamKey)
    {
        // Validate the stream key
        var streams = await _streamRepository.GetAllAsync();

        // Find the stream with the given key by matching against StreamKey property
        var stream = streams.FirstOrDefault(s => s.StreamKey == streamKey);

        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with key {streamKey} not found");
        }

        // Stream has ended, update properties if needed
        // We don't need to remove the StreamUrl or StreamKey, just mark the stream as inactive
        stream.Views = 0; // Reset views or any other properties as needed

        await _streamRepository.UpdateAsync(stream);
        await _cacheService.RemoveAsync("active_streams");
    }

    private StreamDto MapToDto(LiveStream stream, UserDTO? user)
    {
        return new StreamDto
        {
            Id = stream.Id,
            StreamName = stream.StreamName,
            StreamDescription = stream.StreamDescription,
            StreamCategory = stream.StreamCategory,
            Username = user?.Username,
            ThumbnailUrl = stream.ThumbnailUrl,
            StreamUrl = stream.StreamUrl,
            StreamKey = stream.StreamKey,
            Views = stream.Views
        };
    }
}
