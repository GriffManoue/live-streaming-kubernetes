using System.ComponentModel.DataAnnotations;

namespace Shared.Models.User;

public class UpdateUserRequest
{
    public string? Email { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? Phone { get; set; }
    
    [MinLength(8)]
    public string? Password { get; set; }
}
