using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.models.Enums;

namespace Shared.Models.Domain;

public class LiveStream
{
    public Guid Id { get; set; }

    // Add foreign key property
    public Guid UserId { get; set; }

    [Required]
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [Required]
    public required string StreamName { get; set; }

    [Required]
    public required string StreamDescription { get; set; }

    [Required]
    public required StreamCategory StreamCategory { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? StreamUrl { get; set; }
    
    // Add a dedicated property for stream key
    public string? StreamKey { get; set; }

    public int Views { get; set; }

}
