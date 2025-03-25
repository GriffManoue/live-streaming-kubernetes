using System;
using System.ComponentModel.DataAnnotations;
using Shared.models.Enums;

namespace Shared.Models.Domain;

public class LiveStream
{
    public Guid Id { get; set; }

    [Required]
    public virtual User User { get; set; } = null!;

    [Required]
    public required string StreamName { get; set; }

    [Required]
    public required string StreamDescription { get; set; }

    [Required]
    public required StreamCategory StreamCategory { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? StreamUrl { get; set; }

    public int Views { get; set; }

}
