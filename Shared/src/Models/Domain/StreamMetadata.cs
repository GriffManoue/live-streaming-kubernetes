using System;
using System.Collections.Generic;

namespace Shared.Models.Domain;

public class StreamMetadata
{
    public Guid Id { get; set; }
    
    public Guid StreamId { get; set; }
    
    public string? ThumbnailUrl { get; set; }
    
    public int TotalViews { get; set; }
    
    public int LikeCount { get; set; }
    
    public int DislikeCount { get; set; }
    
    public List<string> Tags { get; set; } = new();
    
    // Navigation property
    public virtual LiveStream Stream { get; set; } = null!;
}
