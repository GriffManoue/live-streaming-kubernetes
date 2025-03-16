using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.Interfaces;

namespace StreamService.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthClient _authClient;

    public UserContext(IHttpContextAccessor httpContextAccessor, IAuthClient authClient)
    {
        _httpContextAccessor = httpContextAccessor;
        _authClient = authClient;
    }

    public Guid GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !httpContext.User.Identity?.IsAuthenticated == true)
        {
            return Guid.Empty;
        }

        // Extract user ID from claims
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return Guid.Empty;
        }

        if (Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return Guid.Empty;
    }

    public async Task<bool> ValidateCurrentTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        // Extract the token from the Authorization header
        if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return false;
        }

        var authHeaderValue = authHeader.ToString();
        if (string.IsNullOrEmpty(authHeaderValue) || !authHeaderValue.StartsWith("Bearer "))
        {
            return false;
        }

        var token = authHeaderValue.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        // Validate the token with Auth service
        var result = await _authClient.ValidateTokenAsync(token);
        return result.Success;
    }
} 