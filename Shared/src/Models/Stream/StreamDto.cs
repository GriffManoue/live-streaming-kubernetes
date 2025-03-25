using System;
using Shared.models.Enums;

namespace Shared.Models.Stream;

public class StreamDto
{
    public Guid Id { get; set; }
    public required string StreamName { get; set; }
    public required string StreamDescription { get; set; }
    public required StreamCategory StreamCategory { get; set; }
    public string? Username { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? StreamUrl { get; set; }

    public string? Token { get; set; }
    public int Views { get; set; }
}
