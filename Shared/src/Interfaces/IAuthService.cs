using Shared.Models.Auth;

namespace Shared.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> ValidateTokenAsync(string token);
    Task RevokeTokenAsync(string token);
    Task<AuthResult> GenerateStreamTokenAsync(Guid streamId);
    Task<AuthResult> ValidateStreamTokenAsync(string token);
}