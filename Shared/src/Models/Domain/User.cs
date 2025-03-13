using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Shared.Models.Domain;

public partial class User
{
    private string? _phone;
    
    public Guid Id { get; set; }
    
    [Required] 
    public required string Username { get; set; }
    
    [Required] 
    public required string Password { get; set; }
    
    [Required] 
    public required string Email { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? Phone
    {
        get => _phone;
        set
        {
            if (string.IsNullOrEmpty(value) || !PhoneRegex().IsMatch(value))
                throw new ArgumentException("Invalid phone number format");
            _phone = value;
        }
    }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<LiveStream> Streams { get; set; } = new List<LiveStream>();
    
    public virtual ICollection<UserRelationship> FollowedByRelationships { get; set; } = new List<UserRelationship>();
    
    public virtual ICollection<UserRelationship> FollowingRelationships { get; set; } = new List<UserRelationship>();

    [GeneratedRegex(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")]
    private static partial Regex PhoneRegex();
}
