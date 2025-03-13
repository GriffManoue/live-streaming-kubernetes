using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Shared.models;

public partial class User
{
    private string _phone;
    [Required] public required string Username { get; set; }
    [Required] public required string Password { get; set; }
    [Required] public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string Phone
    {
        get => _phone;
        set
        {
            if (string.IsNullOrEmpty(value) || !PhoneRegex().IsMatch(value))
                throw new ArgumentException("Invalid phone number format");
            _phone = value;
        }
    }

    [GeneratedRegex(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")]
    private static partial Regex PhoneRegex();
}