using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Auth;

public class RegisterRequest
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [MinLength(8)]
    public required string Password { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? Phone { get; set; }
}
