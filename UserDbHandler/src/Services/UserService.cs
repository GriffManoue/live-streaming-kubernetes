using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models.Domain;

namespace UserDbHandler.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    public UserService(IRepository<User> userRepository, IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }
    
    public async Task FollowUserAsync(Guid followerId, Guid followingId)
    {
        if (followerId == followingId)
            throw new ArgumentException("Users cannot follow themselves");

        var follower = await _userRepository.GetByIdWithIncludesAsync(followerId, u => u.Following)
            ?? throw new KeyNotFoundException($"Follower with ID {followerId} not found");
        
        var following = await _userRepository.GetByIdAsync(followingId)
            ?? throw new KeyNotFoundException($"User to follow with ID {followingId} not found");

        // Check if already following
        if (follower.Following.Any(u => u.Id == followingId))
            return; // Already following, nothing to do

        follower.Following.Add(following);
        await _userRepository.UpdateAsync(follower);
    }

    public async Task<IEnumerable<UserDTO>> GetFollowersAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdWithIncludesAsync(userId, u => u.Followers)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found");

        return user.Followers.Select(MapToUserDTO);
    }

    public async Task<IEnumerable<UserDTO>> GetFollowingAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdWithIncludesAsync(userId, u => u.Following)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found");

        return user.Following.Select(MapToUserDTO);
    }

    public async Task<UserDTO> GetUserByIdAsync(Guid id)
    {
        Console.WriteLine($"Fetching user with ID: {id}");
        if(id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(id));
        
        var user = await _userRepository.GetByIdWithIncludesAsync(id, u => u.Followers, u => u.Following)
            ?? throw new KeyNotFoundException($"User with ID {id} not found");

        return MapToUserDTO(user);
    }

    public async Task<UserDTO> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
                u => u.Username == username, 
                u => u.Followers, 
                u => u.Following)
            ?? throw new KeyNotFoundException($"User with username {username} not found");

        return MapToUserDTO(user);
    }

    public async Task UnfollowUserAsync(Guid followerId, Guid followingId)
    {
        if (followerId == followingId)
            throw new ArgumentException("Users cannot unfollow themselves");

        var follower = await _userRepository.GetByIdWithIncludesAsync(followerId, u => u.Following)
            ?? throw new KeyNotFoundException($"Follower with ID {followerId} not found");
        
        var following = await _userRepository.GetByIdAsync(followingId)
            ?? throw new KeyNotFoundException($"User to unfollow with ID {followingId} not found");

        // Check if actually following
        var followingUser = follower.Following.FirstOrDefault(u => u.Id == followingId);
        if (followingUser == null)
            return; // Not following, nothing to do

        follower.Following.Remove(followingUser);
        await _userRepository.UpdateAsync(follower);
    }

    public async Task<UserDTO> UpdateUserAsync(UserDTO userDto)
    {
        var user = await _userRepository.GetByIdAsync(userDto.Id)
            ?? throw new KeyNotFoundException($"User with ID {userDto.Id} not found");

        // Only hash the password if it has changed (i.e., if the provided password does not match the stored hash)
        if (!_passwordHasher.VerifyPassword(userDto.Password, user.Password))
        {
            user.Password = _passwordHasher.HashPassword(userDto.Password);
        }
        // If the password matches the hash, keep the existing hash

        // Update user properties (except relationships)
        user.Username = userDto.Username;
        user.Email = userDto.Email;
        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.IsLive = userDto.IsLive;
        
        // Update the user in the database
        await _userRepository.UpdateAsync(user);
        
        // Fetch the updated user with relationships
        var updatedUser = await _userRepository.GetByIdWithIncludesAsync(
            user.Id, 
            u => u.Followers, 
            u => u.Following);
            
        return MapToUserDTO(updatedUser!);
    }

    // Helper method to map from User to UserDTO
    private static UserDTO MapToUserDTO(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Password = user.Password, 
            Email = user.Email,
            StreamId = user.Stream?.Id ?? Guid.Empty, // Handle null Stream
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            IsLive = user.IsLive,
            FollowerIds = user.Followers?.Select(f => f.Id).ToList() ?? new List<Guid>(),
            FollowingIds = user.Following?.Select(f => f.Id).ToList() ?? new List<Guid>()
        };
    }
}
