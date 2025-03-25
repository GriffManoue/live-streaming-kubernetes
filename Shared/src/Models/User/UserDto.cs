using System;

namespace Shared.Models.User;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
}
