using System;

namespace Shared.Models.Auth;

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Error { get; set; }
    public Guid? UserId { get; set; }
}
