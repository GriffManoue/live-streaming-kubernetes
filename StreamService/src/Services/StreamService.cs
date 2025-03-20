

using Shared.Interfaces;
using Shared.Models.Domain;
using Shared.Models.Stream;
using Shared.Models.User;

namespace StreamService.Services;

public class StreamService : IStreamService
{
    private readonly IRepository<LiveStream> _streamRepository;
    private readonly IRepository<StreamMetadata> _metadataRepository;
    private readonly ICacheService _cacheService;
    private readonly IUserServiceClient _userServiceClient;
    private readonly IUserContext _userContext;

    public StreamService(
        IRepository<LiveStream> streamRepository,
        IRepository<StreamMetadata> metadataRepository,
        ICacheService cacheService,
        IUserServiceClient userServiceClient,
        IUserContext userContext)
    {
        _streamRepository = streamRepository;
        _metadataRepository = metadataRepository;
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
        var activeStreams = streams.Where(s => s.IsActive).ToList();
        
        var result = new List<StreamDto>();
        foreach (var stream in activeStreams)
        {
            var user = await _userServiceClient.GetUserByIdAsync(stream.UserId);
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
        var userStreams = streams.Where(s => s.UserId == userId).ToList();
        
        return userStreams.Select(s => MapToDto(s, user));
    }
    
    public async Task<StreamDto> CreateStreamAsync(CreateStreamRequest request)
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
        
        // Check if user already has an active stream
        var streams = await _streamRepository.GetAllAsync();
        if (streams.Any(s => s.UserId == userId && s.IsActive))
        {
            throw new InvalidOperationException("User already has an active stream");
        }
        
        // Create new stream
        var stream = new LiveStream
        {
            Id = Guid.NewGuid(),
            StreamName = request.StreamName,
            StreamDescription = request.StreamDescription,
            StreamCategory = request.StreamCategory,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            ViewerCount = 0
        };
        
        await _streamRepository.AddAsync(stream);
        
        // Create metadata
        var metadata = new StreamMetadata
        {
            Id = Guid.NewGuid(),
            StreamId = stream.Id,
            TotalViews = 0,
            LikeCount = 0,
            DislikeCount = 0,
            Tags = new List<string>()
        };
        
        await _metadataRepository.AddAsync(metadata);
        
        // Invalidate cache
        await _cacheService.RemoveAsync("active_streams");
        
        return MapToDto(stream, user);
    }
    
    public async Task<StreamDto> UpdateStreamAsync(Guid id, UpdateStreamRequest request)
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
        
        if (stream.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("User is not authorized to update this stream");
        }
        
        // Update stream properties
        if (!string.IsNullOrEmpty(request.StreamName))
        {
            stream.StreamName = request.StreamName;
        }
        
        if (!string.IsNullOrEmpty(request.StreamDescription))
        {
            stream.StreamDescription = request.StreamDescription;
        }
        
        if (request.StreamCategory != null)
        {
            stream.StreamCategory = request.StreamCategory.Value;
        }
        
        await _streamRepository.UpdateAsync(stream);
        
        // Invalidate cache
        await _cacheService.RemoveAsync("active_streams");
        
        var user = await _userServiceClient.GetUserByIdAsync(stream.UserId);
        
        return MapToDto(stream, user);
    }
    
    public async Task EndStreamAsync(Guid id)
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
        
        if (stream.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("User is not authorized to end this stream");
        }
        
        // End the stream
        stream.IsActive = false;
        stream.EndedAt = DateTime.UtcNow;
        
        await _streamRepository.UpdateAsync(stream);
        
        // Invalidate cache
        await _cacheService.RemoveAsync("active_streams");
    }
    
    private StreamDto MapToDto(LiveStream stream, UserDto? user)
    {
        return new StreamDto
        {
            Id = stream.Id,
            StreamName = stream.StreamName,
            StreamDescription = stream.StreamDescription,
            StreamCategory = stream.StreamCategory,
            UserId = stream.UserId,
            Username = user?.Username,
            CreatedAt = stream.CreatedAt,
            EndedAt = stream.EndedAt,
            IsActive = stream.IsActive,
            ViewerCount = stream.ViewerCount
        };
    }
}