using Shared.Interfaces;

namespace Shared.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        // DEBUG LOGGING - REMOVE IN PRODUCTION
        Console.WriteLine($"[DEBUG] Verifying password. Plain: '{password}', Hash: '{hash}'");
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
