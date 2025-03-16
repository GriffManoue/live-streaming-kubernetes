using System.Threading.Tasks;
using Shared.Models.Auth;

namespace StreamService.Services;

public interface IAuthClient
{
    /// <summary>
    /// Validates a JWT token by calling the Auth service
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>Authentication result with success status</returns>
    Task<AuthResult> ValidateTokenAsync(string token);
} 