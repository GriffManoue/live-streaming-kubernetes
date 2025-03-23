using System;
using System.Security.Claims;
using Shared.Models.Domain;

namespace Shared.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateStreamToken(User user, Guid streamId);
    bool ValidateToken(string token);
    bool ValidateStreamToken(string token);
    ClaimsPrincipal GetPrincipalFromToken(string token);
}
