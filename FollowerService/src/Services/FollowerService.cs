using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    }    public async Task FollowUserAsync(Guid followerId, Guid followingId)
    {
        if (followerId == followingId)
            throw new ArgumentException("Users cannot follow themselves");

        try
        {
            // Use the direct follow method from UserDbHandlerClient
            await _userDbHandlerClient.FollowUserAsync(followerId, followingId);
            _logger.LogInformation("User {FollowerId} successfully followed user {FollowingId}", followerId, followingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when user {FollowerId} tried to follow user {FollowingId}", followerId, followingId);
            throw;
        }
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
    }    public async Task UnfollowUserAsync(Guid followerId, Guid followingId)
    {
        if (followerId == followingId)
            throw new ArgumentException("Users cannot unfollow themselves");

        try
        {
            // Use the direct unfollow method from UserDbHandlerClient
            await _userDbHandlerClient.UnfollowUserAsync(followerId, followingId);
            _logger.LogInformation("User {FollowerId} successfully unfollowed user {FollowingId}", followerId, followingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when user {FollowerId} tried to unfollow user {FollowingId}", followerId, followingId);
            throw;
        }
    }
}
