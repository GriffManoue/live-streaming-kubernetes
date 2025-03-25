

namespace Shared.Interfaces;

public interface IUserService
{
    Task<UserDTO> GetUserByIdAsync(Guid id);
    Task<UserDTO> GetUserByUsernameAsync(string username);
    Task<UserDTO> UpdateUserAsync(UserDTO user);
    Task<IEnumerable<UserDTO>> GetFollowersAsync(Guid userId);
    Task<IEnumerable<UserDTO>> GetFollowingAsync(Guid userId);
    Task FollowUserAsync(Guid followerId, Guid followingId);
    Task UnfollowUserAsync(Guid followerId, Guid followingId);
}
