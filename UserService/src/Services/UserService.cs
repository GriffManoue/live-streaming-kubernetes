using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;
using Shared.Models.Domain;
using Shared.Models.User;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserRelationship> _relationshipRepository;
    
    public UserService(IRepository<User> userRepository, IRepository<UserRelationship> relationshipRepository)
    {
        _userRepository = userRepository;
        _relationshipRepository = relationshipRepository;
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
        
        if (!string.IsNullOrEmpty(request.Phone))
        {
            user.Phone = request.Phone;
        }
        
        // Update password if provided
        if (!string.IsNullOrEmpty(request.Password))
        {
            // In a real implementation, you would hash the password here
            user.Password = request.Password;
        }
        
        await _userRepository.UpdateAsync(user);
        
        return MapToDto(user);
    }
    
    public async Task<IEnumerable<UserDto>> GetFollowersAsync(Guid userId)
    {
        var relationships = await _relationshipRepository.GetAllAsync();
        var followerRelationships = relationships.Where(r => r.FollowingId == userId).ToList();
        
        var followers = new List<UserDto>();
        foreach (var relationship in followerRelationships)
        {
            var follower = await _userRepository.GetByIdAsync(relationship.FollowerId);
            if (follower != null)
            {
                followers.Add(MapToDto(follower));
            }
        }
        
        return followers;
    }
    
    public async Task<IEnumerable<UserDto>> GetFollowingAsync(Guid userId)
    {
        var relationships = await _relationshipRepository.GetAllAsync();
        var followingRelationships = relationships.Where(r => r.FollowerId == userId).ToList();
        
        var following = new List<UserDto>();
        foreach (var relationship in followingRelationships)
        {
            var followedUser = await _userRepository.GetByIdAsync(relationship.FollowingId);
            if (followedUser != null)
            {
                following.Add(MapToDto(followedUser));
            }
        }
        
        return following;
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
        var relationships = await _relationshipRepository.GetAllAsync();
        var existingRelationship = relationships.FirstOrDefault(r => 
            r.FollowerId == followerId && r.FollowingId == followingId);
        
        if (existingRelationship != null)
        {
            // Already following
            return;
        }
        
        // Create new relationship
        var relationship = new UserRelationship
        {
            Id = Guid.NewGuid(),
            FollowerId = followerId,
            FollowingId = followingId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _relationshipRepository.AddAsync(relationship);
    }
    
    public async Task UnfollowUserAsync(Guid followerId, Guid followingId)
    {
        // Find the relationship
        var relationships = await _relationshipRepository.GetAllAsync();
        var relationship = relationships.FirstOrDefault(r => 
            r.FollowerId == followerId && r.FollowingId == followingId);
        
        if (relationship == null)
        {
            // Not following, nothing to do
            return;
        }
        
        // Delete the relationship
        await _relationshipRepository.DeleteAsync(relationship.Id);
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
            Phone = user.Phone,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            FollowersCount = user.FollowedByRelationships.Count,
            FollowingCount = user.FollowingRelationships.Count
        };
    }
}
