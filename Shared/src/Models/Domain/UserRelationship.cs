using System;

namespace Shared.Models.Domain;

public class UserRelationship
{
    public Guid Id { get; set; }
    
    public Guid FollowerId { get; set; }
    
    public Guid FollowingId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User Follower { get; set; } = null!;
    
    public virtual User Following { get; set; } = null!;
}
