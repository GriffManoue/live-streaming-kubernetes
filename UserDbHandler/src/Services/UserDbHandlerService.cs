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

        List<User> followers = new List<User>();
        List<User> following = new List<User>();

        foreach (var followerId in userDto.FollowerIds)
        {
            var follower = await _userRepository.GetByIdAsync(followerId);
            if (follower != null)
            {
                followers.Add(follower);
            }
        }
        foreach (var followingId in userDto.FollowingIds)
        {
            var followingUser = await _userRepository.GetByIdAsync(followingId);
            if (followingUser != null)
            {
                following.Add(followingUser);
            }
        }

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
