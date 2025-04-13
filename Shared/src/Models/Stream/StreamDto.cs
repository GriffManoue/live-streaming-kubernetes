using Shared.models.Enums;

namespace Shared.Models.Stream;

public class StreamDto
{
    public Guid Id { get; set; }
    public string StreamName { get; set; } = string.Empty;
    public string StreamDescription { get; set; } = string.Empty;
    public StreamCategory StreamCategory { get; set; }
    public string? Username { get; set; }
    public Guid? UserId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? StreamUrl { get; set; }
    public string? StreamKey { get; set; }
    public int Views { get; set; }
}