using System;
using System.ComponentModel.DataAnnotations;
using Shared.models.Enums;

namespace Shared.Models.Domain;

public class LiveStream
{
    public Guid Id { get; set; }
    
    [Required] 
    public required string StreamName { get; set; }
    
    [Required] 
    public required string StreamDescription { get; set; }
    
    [Required] 
    public required StreamCategory StreamCategory { get; set; }
    
    public Guid UserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndedAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int ViewerCount { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    
    public virtual StreamMetadata? Metadata { get; set; }
}
