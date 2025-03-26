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
        IUserContext userContext,
        ITokenService tokenService)
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
        
        var user = await _userServiceClient.GetUserByIdAsync(stream.User.Id);
        
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
            var user = await _userServiceClient.GetUserByIdAsync(stream.User.Id);
            result.Add(MapToDto(stream, user));
        }
        
        // Cache the result
        await _cacheService.SetAsync("active_streams", result, TimeSpan.FromMinutes(1));
        
        return result;
    }
    
    public async Task<IEnumerable<StreamDto>> GetStreamsByUserIdAsync(Guid userId)
    {
        var user = await _userServiceClient.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }
        
        var streams = await _streamRepository.GetAllAsync();
        var userStreams = streams.Where(s => s.User.Id == userId).ToList();
        
        return userStreams.Select(s => MapToDto(s, user));
    }
     
    public async Task<StreamDto> CreateStreamAsync()
    {
        // Validate the user's token
        if (!await _userContext.ValidateCurrentTokenAsync())
        {
            throw new UnauthorizedAccessException("Invalid or expired authentication token");
        }

        // Get the authenticated user's ID
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        
        var user = await _userServiceClient.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }
        
        // Check if user already has a stream
        var streams = await _streamRepository.GetAllAsync();
        if (streams.Any(s => s.User.Id == userId))
        {
            throw new InvalidOperationException("User already has an active stream");
        }
        
        // Create new stream
        var stream = new LiveStream
        {
            Id = Guid.NewGuid(),
            StreamName = "New Stream",
            StreamDescription = "Stream Description",
            StreamCategory = StreamCategory.Gaming,
            ThumbnailUrl = "",
            StreamUrl = "",
            User = new User
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Password = "notneeded",
            },
        };
        
        await _streamRepository.AddAsync(stream);
        return MapToDto(stream, user);
    }
    
    public async Task<StreamDto> UpdateStreamAsync(Guid id, StreamDto streamDto)
    {
        // Validate the user's token
        if (!await _userContext.ValidateCurrentTokenAsync())
        {
            throw new UnauthorizedAccessException("Invalid or expired authentication token");
        }

        var stream = await _streamRepository.GetByIdAsync(id);
        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with ID {id} not found");
        }
        
        // Check if the current user is the owner of the stream
        var currentUserId = _userContext.GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        
        if (stream.User.Id != currentUserId)
        {
            throw new UnauthorizedAccessException("User is not authorized to update this stream");
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
        
        var user = await _userServiceClient.GetUserByIdAsync(stream.User.Id);
        
        return MapToDto(stream, user);
    }
    
    public async Task EndStreamAsync(Guid id)
    {
        var stream = await _streamRepository.GetByIdAsync(id);
        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with ID {id} not found");
        }
        
        await _streamRepository.UpdateAsync(stream);
        
        // Invalidate cache
        await _cacheService.RemoveAsync("active_streams");
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
            Views = stream.Views
        };
    }
}
