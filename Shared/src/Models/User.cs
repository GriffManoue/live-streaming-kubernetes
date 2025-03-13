using System.Text.RegularExpressions;

namespace Shared.models;

public partial class User
{
	[Required]
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    private string _phone;
    public string Phone 
    { 
        get => _phone; 
        set 
        {
            if (string.IsNullOrEmpty(value) || !PhoneRegex().IsMatch(value))
            {
                throw new ArgumentException("Invalid phone number format");
            }
            _phone = value;
        }
    }

    [GeneratedRegex(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")]
    private static partial Regex PhoneRegex();
}