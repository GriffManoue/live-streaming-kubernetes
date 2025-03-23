using System;
using System.Collections.Generic;

namespace Shared.Models.Domain;

public class StreamMetadata
{
    public Guid Id { get; set; }
    
    public Guid StreamId { get; set; }
    
    public string? ThumbnailUrl { get; set; }
    
    public int Views { get; set; }

    public bool IsLive { get; set; } = false;
    
    // Navigation property
    public virtual LiveStream Stream { get; set; } = null!;
}
