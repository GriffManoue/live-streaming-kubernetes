using System.Threading.Tasks;
using Shared.Models.Auth;

namespace Shared.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> ValidateTokenAsync(string token);
    Task RevokeTokenAsync(string token);
}
