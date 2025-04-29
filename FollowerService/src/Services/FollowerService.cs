using System;
using Shared.src.Interfaces.Services;
using Shared.src.Models.User;
using StreamDbHandler.Services;

namespace FollowerService.src.Services;

public class FollowerService : IFollowerService
{
    private readonly IUserDbHandlerClient _userDbHandlerClient;
    private readonly ILogger<FollowerService> _logger;

    public FollowerService(IUserDbHandlerClient userDbHandlerClient, ILogger<FollowerService> logger)
    {
        _userDbHandlerClient = userDbHandlerClient;
        _logger = logger;
    }

    public async Task FollowUserAsync(Guid followerId, Guid followingId)
    {
        if (followerId == followingId)
            throw new ArgumentException("Users cannot follow themselves");

        // Get the user to be followed
        var following = await _userDbHandlerClient.GetUserByIdWithFollowersAsync(followingId)
            ?? throw new KeyNotFoundException($"User to follow with ID {followingId} not found");

        // If already followed, do nothing
        if (following.User.FollowerIds.Contains(followerId))
            return;

        following.User.FollowerIds.Add(followerId);
        await _userDbHandlerClient.UpdateUserAsync(followingId, following.User);
    }

    public async Task<IEnumerable<UserDTO>> GetFollowersAsync(Guid userId)
    {
        var user = await _userDbHandlerClient.GetUserByIdWithFollowersAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found");
        return user.Followers;
    }

    public async Task<IEnumerable<UserDTO>> GetFollowingAsync(Guid userId)
    {
        var user = await _userDbHandlerClient.GetUserByIdWithFollowersAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found");
        return user.Following;
    }

    public async Task UnfollowUserAsync(Guid followerId, Guid followingId)
    {
        if (followerId == followingId)
            throw new ArgumentException("Users cannot unfollow themselves");

        // Get the user to be unfollowed
        var following = await _userDbHandlerClient.GetUserByIdWithFollowersAsync(followingId)
            ?? throw new KeyNotFoundException($"User to unfollow with ID {followingId} not found");

        if (!following.User.FollowerIds.Contains(followerId))
            return;

        following.User.FollowerIds.Remove(followerId);
        await _userDbHandlerClient.UpdateUserAsync(followingId, following.User);
    }
}
