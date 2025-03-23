using Shared.Interfaces;
using Shared.Models.Domain;
using Shared.Models.Stream;
using Shared.Models.User;

namespace RecommendationService.Services;

public class RecommendationService : IRecommendationService
{
    private readonly ICacheService _cacheService;
    private readonly IRepository<UserRelationship> _relationshipRepository;
    private readonly IRepository<LiveStream> _streamRepository;
    private readonly IRepository<User> _userRepository;

    public RecommendationService(
        IRepository<LiveStream> streamRepository,
        IRepository<User> userRepository,
        IRepository<UserRelationship> relationshipRepository,
        ICacheService cacheService)
    {
        _streamRepository = streamRepository;
        _userRepository = userRepository;
        _relationshipRepository = relationshipRepository;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<StreamDto>> GetRecommendedStreamsAsync(Guid userId)
    {
        // Try to get from cache first
        var cacheKey = $"recommended_streams_{userId}";
        var cachedRecommendations = await _cacheService.GetAsync<List<StreamDto>>(cacheKey);
        if (cachedRecommendations != null) return cachedRecommendations;

        // Get user's viewing history and preferences
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException($"User with ID {userId} not found");

        // Get active streams
        var allStreams = await _streamRepository.GetAllAsync();
        var activeStreams = allStreams.Where(s => s.IsActive).ToList();

        // Get user's followed streamers
        var allRelationships = await _relationshipRepository.GetAllAsync();
        var followedUsers = allRelationships.Where(r => r.FollowerId == userId).ToList();
        var followedUserIds = followedUsers.Select(f => f.FollowingId).ToHashSet();

        // In a real implementation, this would use a machine learning model
        // For now, we'll use a simple recommendation algorithm:
        // 1. Streams from followed users get highest priority
        // 2. Popular streams in categories the user has watched before
        // 3. Generally popular streams

        var recommendations = new List<StreamDto>();

        // First, add streams from followed users
        foreach (var stream in activeStreams.Where(s => followedUserIds.Contains(s.UserId)))
            recommendations.Add(new StreamDto
            {
                Id = stream.Id,
                StreamName = stream.StreamName,
                StreamDescription = stream.StreamDescription,
                StreamCategory = stream.StreamCategory,
                UserId = stream.UserId,
                Username = stream.User.Username,
                IsActive = stream.IsActive,
            });

        // Then add other popular streams (limited to 10 total recommendations)
        foreach (var stream in activeStreams
                     .Where(s => !followedUserIds.Contains(s.UserId))
                     .Take(10 - recommendations.Count))
            recommendations.Add(new StreamDto
            {
                Id = stream.Id,
                StreamName = stream.StreamName,
                StreamDescription = stream.StreamDescription,
                StreamCategory = stream.StreamCategory,
                UserId = stream.UserId,
                Username = stream.User.Username,
                IsActive = stream.IsActive,
            });

        // Cache the result
        await _cacheService.SetAsync(cacheKey, recommendations, TimeSpan.FromMinutes(5));

        return recommendations;
    }

    public async Task<IEnumerable<UserDto>> GetRecommendedUsersToFollowAsync(Guid userId)
    {
        // Try to get from cache first
        var cacheKey = $"recommended_users_{userId}";
        var cachedRecommendations = await _cacheService.GetAsync<List<UserDto>>(cacheKey);
        if (cachedRecommendations != null) return cachedRecommendations;

        // Get user's following relationships
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException($"User with ID {userId} not found");

        // Get user's followed users
        var allRelationships = await _relationshipRepository.GetAllAsync();
        var followedUsers = allRelationships.Where(r => r.FollowerId == userId).ToList();
        var followedUserIds = followedUsers.Select(f => f.FollowingId).ToHashSet();

        // Get users who are followed by users that the current user follows (friends of friends)
        var friendsOfFriends = new HashSet<Guid>();
        foreach (var followedId in followedUserIds)
        {
            var friendsOfFriend = allRelationships.Where(r => r.FollowerId == followedId).ToList();
            foreach (var friend in friendsOfFriend)
                if (!followedUserIds.Contains(friend.FollowingId) && friend.FollowingId != userId)
                    friendsOfFriends.Add(friend.FollowingId);
        }

        // Get popular streamers
        var allUsers = await _userRepository.GetAllAsync();
        var popularStreamers = allUsers.Where(u => u.Streams.Any(s => s.IsActive)).ToList();

        var recommendations = new List<UserDto>();

        // First, add friends of friends
        foreach (var friendId in friendsOfFriends.Take(5))
        {
            var friend = await _userRepository.GetByIdAsync(friendId);
            if (friend != null)
                recommendations.Add(new UserDto
                {
                    Id = friend.Id,
                    Username = friend.Username,
                    Email = friend.Email,
                    FirstName = friend.FirstName,
                    LastName = friend.LastName,
                    CreatedAt = friend.CreatedAt
                });
        }

        // Then add popular streamers (limited to 10 total recommendations)
        foreach (var streamer in popularStreamers
                     .Where(s => !followedUserIds.Contains(s.Id) && !friendsOfFriends.Contains(s.Id) && s.Id != userId)
                     .Take(10 - recommendations.Count))
            recommendations.Add(new UserDto
            {
                Id = streamer.Id,
                Username = streamer.Username,
                Email = streamer.Email,
                FirstName = streamer.FirstName,
                LastName = streamer.LastName,
                CreatedAt = streamer.CreatedAt
            });

        // Cache the result
        await _cacheService.SetAsync(cacheKey, recommendations, TimeSpan.FromMinutes(15));

        return recommendations;
    }
}