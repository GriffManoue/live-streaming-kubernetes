using System;

namespace Shared.src.Interfaces.Services;

public interface IFollowerService
{
    Task<IEnumerable<UserDTO>> GetFollowersAsync(Guid userId);
    Task<IEnumerable<UserDTO>> GetFollowingAsync(Guid userId);
    Task FollowUserAsync(Guid followerId, Guid followingId);
    Task UnfollowUserAsync(Guid followerId, Guid followingId);
}
