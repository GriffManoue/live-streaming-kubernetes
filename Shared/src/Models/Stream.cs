using System.ComponentModel.DataAnnotations;

namespace Shared.models;

public class Stream
{
    [Required] public required User User { get; set; }
    [Required] public required string StreamName { get; set; }
    [Required] public required string StreamDescription { get; set; }
    [Required] public required string StreamCategory { get; set; }
}