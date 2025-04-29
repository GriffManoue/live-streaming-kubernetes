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

        var follower = await _userDbHandlerClient.GetUserByIdWithFollowersAsync(followerId)
            ?? throw new KeyNotFoundException($"Follower with ID {followerId} not found");
        var following = await _userDbHandlerClient.GetUserByIdWithFollowersAsync(followingId)
            ?? throw new KeyNotFoundException($"User to follow with ID {followingId} not found");

        // Check if already following
        if (follower.Following.Any(u => u.Id == followingId))
            return; // Already following, nothing to do

        // Update follower's Following list
        follower.User.FollowerIds.Add(followingId);
        await _userDbHandlerClient.UpdateUserAsync(followerId, follower.User);

        // Update followed user's Followers list
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

        var follower = await _userDbHandlerClient.GetUserByIdWithFollowersAsync(followerId)
            ?? throw new KeyNotFoundException($"Follower with ID {followerId} not found");
        var following = await _userDbHandlerClient.GetUserByIdAsync(followingId)
            ?? throw new KeyNotFoundException($"User to unfollow with ID {followingId} not found");

        var followingList = follower.Following.ToList();
        var toRemove = followingList.FirstOrDefault(u => u.Id == followingId);
        if (toRemove == null)
            return; // Not following, nothing to do
        followingList.Remove(toRemove);
        follower = new UserWithFollowersDTO(follower.User, follower.Followers, followingList);
        await _userDbHandlerClient.UpdateUserAsync(followerId, follower.User);
    }
}
