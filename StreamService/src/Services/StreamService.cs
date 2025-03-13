using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models.Domain;
using Shared.Models.Stream;

namespace StreamService.Services;

public class StreamService : IStreamService
{
    private readonly IRepository<LiveStream> _streamRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<StreamMetadata> _metadataRepository;
    private readonly ICacheService _cacheService;
    
    public StreamService(
        IRepository<LiveStream> streamRepository,
        IRepository<User> userRepository,
        IRepository<StreamMetadata> metadataRepository,
        ICacheService cacheService)
    {
        _streamRepository = streamRepository;
        _userRepository = userRepository;
        _metadataRepository = metadataRepository;
        _cacheService = cacheService;
    }
    
    public async Task<StreamDto> GetStreamByIdAsync(Guid id)
    {
        var stream = await _streamRepository.GetByIdAsync(id);
        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with ID {id} not found");
        }
        
        var user = await _userRepository.GetByIdAsync(stream.UserId);
        
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
            var user = await _userRepository.GetByIdAsync(stream.UserId);
            if (user != null)
            {
                result.Add(MapToDto(stream, user));
            }
        }
        
        // Cache the result
        await _cacheService.SetAsync("active_streams", result, TimeSpan.FromMinutes(1));
        
        return result;
    }
    
    public async Task<IEnumerable<StreamDto>> GetStreamsByUserIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
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
        // Get the user from the token (in a real implementation, this would come from the authenticated user)
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Placeholder
        
        var user = await _userRepository.GetByIdAsync(userId);
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
        var stream = await _streamRepository.GetByIdAsync(id);
        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with ID {id} not found");
        }
        
        // In a real implementation, check if the user is the owner of the stream
        
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
        
        var user = await _userRepository.GetByIdAsync(stream.UserId);
        
        return MapToDto(stream, user);
    }
    
    public async Task EndStreamAsync(Guid id)
    {
        var stream = await _streamRepository.GetByIdAsync(id);
        if (stream == null)
        {
            throw new KeyNotFoundException($"Stream with ID {id} not found");
        }
        
        // In a real implementation, check if the user is the owner of the stream
        
        // End the stream
        stream.IsActive = false;
        stream.EndedAt = DateTime.UtcNow;
        
        await _streamRepository.UpdateAsync(stream);
        
        // Invalidate cache
        await _cacheService.RemoveAsync("active_streams");
    }
    
    private StreamDto MapToDto(LiveStream stream, User? user)
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
