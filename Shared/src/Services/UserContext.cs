using Shared.Interfaces;
using Microsoft.AspNetCore.Http;


namespace Shared.Services;

public class UserContext : IUserContext
{
    private readonly ICacheService _cacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor, ICacheService cacheService)
    {
        _httpContextAccessor = httpContextAccessor;
        _cacheService = cacheService;
    }

    public Guid GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return Guid.Empty;

        var userIdClaim = httpContext.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Guid.Empty;

        return userId;
    }

    public async Task<bool> ValidateCurrentTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return false;

        var token = httpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token)) return false;

        // Check if token is blacklisted (revoked)
        var isRevoked = await _cacheService.GetAsync<bool>($"revoked_token:{token}");
        return !isRevoked;
    }
}