using System.ComponentModel.DataAnnotations;
using Shared.models.Enums;

namespace Shared.Models.Stream;

public class CreateStreamRequest
{
    [Required]
    public required string StreamName { get; set; }
    
    [Required]
    public required string StreamDescription { get; set; }
    
    [Required]
    public required StreamCategory StreamCategory { get; set; }
}
