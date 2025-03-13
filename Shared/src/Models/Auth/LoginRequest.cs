using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Auth;

public class LoginRequest
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string Password { get; set; }
}
