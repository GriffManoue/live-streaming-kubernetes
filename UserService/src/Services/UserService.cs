using Shared.Interfaces;
using Shared.Models.Domain;
using Shared.Models.User;
using BCrypt.Net;


namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    
    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }
        
        return MapToDto(user);
    }
    
    public async Task<UserDto> GetUserByUsernameAsync(string username)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        
        if (user == null)
        {
            throw new KeyNotFoundException($"User with username {username} not found");
        }
        
        return MapToDto(user);
    }
    
    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }
        
        // Update user properties
        if (!string.IsNullOrEmpty(request.Email))
        {
            user.Email = request.Email;
        }
        
        if (!string.IsNullOrEmpty(request.FirstName))
        {
            user.FirstName = request.FirstName;
        }
        
        if (!string.IsNullOrEmpty(request.LastName))
        {
            user.LastName = request.LastName;
        }
        
        if (!string.IsNullOrEmpty(request.Password))
        {
            user.Password =BCrypt.Net.BCrypt.HashPassword(request.Password);
        }
        
        await _userRepository.UpdateAsync(user);
        
        return MapToDto(user);
    }
    
    public async Task<IEnumerable<UserDto>> GetFollowersAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        return user.Followers
            .Select(f => new UserDto
            {
                Id = f.Id,
                Username = f.Username,
                Email = f.Email,
                FirstName = f.FirstName,
                LastName = f.LastName,
                CreatedAt = f.CreatedAt,
                FollowersCount = f.Followers.Count,
                FollowingCount = f.Following.Count
            })
            .ToList();
    }
    
    public async Task<IEnumerable<UserDto>> GetFollowingAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        return user.Following
            .Select(f => new UserDto
            {
                Id = f.Id,
                Username = f.Username,
                Email = f.Email,
                FirstName = f.FirstName,
                LastName = f.LastName,
                CreatedAt = f.CreatedAt,
                FollowersCount = f.Followers.Count,
                FollowingCount = f.Following.Count
            })
            .ToList();
    }
    
    public async Task FollowUserAsync(Guid followerId, Guid followingId)
    {
        // Check if users exist
        var follower = await _userRepository.GetByIdAsync(followerId);
        var following = await _userRepository.GetByIdAsync(followingId);
        
        if (follower == null)
        {
            throw new KeyNotFoundException($"Follower with ID {followerId} not found");
        }
        
        if (following == null)
        {
            throw new KeyNotFoundException($"User to follow with ID {followingId} not found");
        }
        
        // Check if relationship already exists
        if (follower.Following.Any(u => u.Id == followingId))
        {
            // Already following, nothing to do
            return;
        }
        
        // Create new relationship
        follower.Following.Add(following);
        following.Followers.Add(follower);
        
    }
    
    public async Task UnfollowUserAsync(Guid followerId, Guid followingId)
    {
        // Check if users exist
        var follower = await _userRepository.GetByIdAsync(followerId);
        var following = await _userRepository.GetByIdAsync(followingId);
        
        if (follower == null)
        {
            throw new KeyNotFoundException($"Follower with ID {followerId} not found");
        }
        
        if (following == null)
        {
            throw new KeyNotFoundException($"User to unfollow with ID {followingId} not found");
        }
        
        // Check if relationship exists
        if (!follower.Following.Any(u => u.Id == followingId))
        {
            // Not following, nothing to do
            return;
        }
        
        // Remove relationship
        follower.Following.Remove(following);
        following.Followers.Remove(follower);
    }
    
    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            FollowersCount = user.Followers.Count,
            FollowingCount = user.Following.Count
        };
    }
}
