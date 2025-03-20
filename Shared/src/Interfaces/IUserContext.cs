using System;
using System.Threading.Tasks;

namespace Shared.Interfaces;

public interface IUserContext
{
    /// <summary>
    /// Gets the current authenticated user's ID
    /// </summary>
    /// <returns>The user ID, or Guid.Empty if no user is authenticated</returns>
    Guid GetCurrentUserId();
    
    /// <summary>
    /// Validates the current user's token with the Auth service
    /// </summary>
    /// <returns>True if the token is valid, false otherwise</returns>
    Task<bool> ValidateCurrentTokenAsync();
} 