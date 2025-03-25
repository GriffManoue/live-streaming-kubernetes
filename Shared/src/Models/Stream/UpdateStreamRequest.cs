using Shared.models.Enums;

namespace Shared.Models.Stream;

public class UpdateStreamRequest
{
    public string? StreamName { get; set; }
    
    public string? StreamDescription { get; set; }
    
    public StreamCategory? StreamCategory { get; set; }

    public string? ThumbnailUrl { get; set; }
    public string? StreamUrl { get; set; }
    public int? Views { get; set; }
}
