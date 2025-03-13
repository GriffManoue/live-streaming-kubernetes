using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models.User;

namespace Shared.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<UserDto> GetUserByUsernameAsync(string username);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<IEnumerable<UserDto>> GetFollowersAsync(Guid userId);
    Task<IEnumerable<UserDto>> GetFollowingAsync(Guid userId);
    Task FollowUserAsync(Guid followerId, Guid followingId);
    Task UnfollowUserAsync(Guid followerId, Guid followingId);
}
