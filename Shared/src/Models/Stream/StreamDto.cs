using System;
using Shared.models.Enums;

namespace Shared.Models.Stream;

public class StreamDto
{
    public Guid Id { get; set; }
    public required string StreamName { get; set; }
    public required string StreamDescription { get; set; }
    public required StreamCategory StreamCategory { get; set; }
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public bool IsActive { get; set; }
    public int ViewerCount { get; set; }
}
