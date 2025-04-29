using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models.Domain;
using Shared.src.Models.User;

namespace UserDbHandler.Services;

public class UserDbHandlerService : IUserDbHandlerService
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserDbHandlerService(IRepository<User> userRepository, IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }

    public async Task<UserDTO> GetUserByIdAsync(Guid id)
    {
        Console.WriteLine($"Fetching user with ID: {id}");
        if (id == Guid.Empty)
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

    public async Task<UserDTO> UpdateUserAsync(UserDTO userDto)
    {
        var user = await _userRepository.GetByIdAsync(userDto.Id)
            ?? throw new KeyNotFoundException($"User with ID {userDto.Id} not found");

        // Only hash the password if it has changed (i.e., if the provided password does not match the stored hash)
        if (userDto.Password != user.Password)
        {
            Console.WriteLine($"Password for user {userDto.Username} has changed. Hashing new password.");
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

    public Task<UserWithFollowersDTO> GetUserByIdWithIncludesAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(id));

        var user = _userRepository.GetByIdWithIncludesAsync(id, u => u.Followers, u => u.Following)
            ?? throw new KeyNotFoundException($"User with ID {id} not found");
        return user.ContinueWith(t => MapToUserWithFollowersDTO(t.Result!));
    }

    public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
    {
        if (userDto == null) throw new ArgumentNullException(nameof(userDto));
        var user = new User
        {
            Id = userDto.Id == Guid.Empty ? Guid.NewGuid() : userDto.Id,
            Username = userDto.Username,
            Email = userDto.Email,
            Password = _passwordHasher.HashPassword(userDto.Password),
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            IsLive = userDto.IsLive,
            CreatedAt = userDto.CreatedAt == default ? DateTime.UtcNow : userDto.CreatedAt,
            Followers = new List<User>(),
            Following = new List<User>()
        };
        await _userRepository.AddAsync(user);
        return MapToUserDTO(user);
    }

    public async Task<UserDTO?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == email, u => u.Followers, u => u.Following);
        return user == null ? null : MapToUserDTO(user);
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
    }    public async Task FollowUserAsync(Guid followerId, Guid followingId)
    {
        if (followerId == Guid.Empty)
            throw new ArgumentException("Follower ID cannot be empty", nameof(followerId));
        
        if (followingId == Guid.Empty)
            throw new ArgumentException("Following ID cannot be empty", nameof(followingId));
        
        if (followerId == followingId)
            throw new ArgumentException("A user cannot follow themselves", nameof(followerId));
        
        // Get both users with their relationships
        var follower = await _userRepository.GetByIdWithIncludesAsync(followerId, u => u.Following)
            ?? throw new KeyNotFoundException($"Follower with ID {followerId} not found");
        
        var following = await _userRepository.GetByIdWithIncludesAsync(followingId, u => u.Followers)
            ?? throw new KeyNotFoundException($"User to follow with ID {followingId} not found");
        
        // Check if already following
        if (follower.Following.Any(f => f.Id == followingId))
            return; // Already following, no action needed
        
        // Add the following relationship
        follower.Following.Add(following);
        
        // Save changes
        await _userRepository.SaveChangesAsync();
    }

    public async Task UnfollowUserAsync(Guid followerId, Guid followingId)
    {
        if (followerId == Guid.Empty)
            throw new ArgumentException("Follower ID cannot be empty", nameof(followerId));
        
        if (followingId == Guid.Empty)
            throw new ArgumentException("Following ID cannot be empty", nameof(followingId));
        
        if (followerId == followingId)
            throw new ArgumentException("A user cannot unfollow themselves", nameof(followerId));
        
        // Get follower with their following relationships
        var follower = await _userRepository.GetByIdWithIncludesAsync(followerId, u => u.Following)
            ?? throw new KeyNotFoundException($"Follower with ID {followerId} not found");
        
        // Get the user to unfollow
        var following = await _userRepository.GetByIdAsync(followingId)
            ?? throw new KeyNotFoundException($"User to unfollow with ID {followingId} not found");
        
        // Check if actually following
        var existingRelationship = follower.Following.FirstOrDefault(f => f.Id == followingId);
        if (existingRelationship == null)
            return; // Not following, no action needed
        
        // Remove the following relationship
        follower.Following.Remove(existingRelationship);
        
        // Save changes
        await _userRepository.SaveChangesAsync();
    }

    // Helper method to map from User to UserWithFollowersDTO
    private static UserWithFollowersDTO MapToUserWithFollowersDTO(User user)
    {
        return new UserWithFollowersDTO(
            MapToUserDTO(user),
            user.Followers?.Select(MapToUserDTO) ?? Enumerable.Empty<UserDTO>(),
            user.Following?.Select(MapToUserDTO) ?? Enumerable.Empty<UserDTO>()
        );
    }


}
