using System;
using System.Security.Claims;
using Shared.Models.Domain;

namespace Shared.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    ClaimsPrincipal GetPrincipalFromToken(string token);
}
