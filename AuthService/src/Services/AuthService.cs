using Shared.Interfaces;
using Shared.Models.Auth;
using Shared.Models.Domain;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly ICacheService _cacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRepository<User> _userRepository;

    public AuthService(
        IRepository<User> userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IHttpContextAccessor httpContextAccessor,
        ICacheService cacheService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor;
        _cacheService = cacheService;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        // Check if user with the same username or email already exists
        var users = await _userRepository.GetAllAsync();
        if (users.Any(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase)))
            return new AuthResult
            {
                Success = false,
                Error = "Username already exists"
            };

        if (users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            return new AuthResult
            {
                Success = false,
                Error = "Email already exists"
            };

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            Password = _passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user);

        // Generate token
        var token = _tokenService.GenerateToken(user);

        return new AuthResult
        {
            Success = true,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            UserId = user.Id
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        // Find user by username
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u =>
            u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
            return new AuthResult
            {
                Success = false,
                Error = "Invalid username or password"
            };

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.Password))
            return new AuthResult
            {
                Success = false,
                Error = "Invalid username or password"
            };

        // Generate token
        var token = _tokenService.GenerateToken(user);

        return new AuthResult
        {
            Success = true,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            UserId = user.Id
        };
    }

    public async Task<AuthResult> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            return new AuthResult
            {
                Success = false,
                Error = "Token is required"
            };

        // Check if token is in the blacklist
        var blacklistKey = $"revoked_token:{token}";
        var isRevoked = await _cacheService.GetAsync<bool>(blacklistKey);
        if (isRevoked)
            return new AuthResult
            {
                Success = false,
                Error = "Token has been revoked"
            };

        if (!_tokenService.ValidateToken(token))
            return new AuthResult
            {
                Success = false,
                Error = "Invalid or expired token"
            };

        var principal = _tokenService.GetPrincipalFromToken(token);
        var userIdClaim = principal.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return new AuthResult
            {
                Success = false,
                Error = "Invalid token"
            };

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return new AuthResult
            {
                Success = false,
                Error = "User not found"
            };

        return new AuthResult
        {
            Success = true,
            Token = token,
            UserId = userId
        };
    }

    public async Task RevokeTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        // Validate the token first
        if (!_tokenService.ValidateToken(token))
            throw new InvalidOperationException("Cannot revoke an invalid token");

        var principal = _tokenService.GetPrincipalFromToken(token);
        var expirationClaim = principal.FindFirst("exp")?.Value;

        if (string.IsNullOrEmpty(expirationClaim) || !long.TryParse(expirationClaim, out var expirationUnix))
            throw new InvalidOperationException("Token missing expiration claim");

        // Convert Unix timestamp to DateTime
        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationUnix).UtcDateTime;
        var timeUntilExpiration = expirationTime - DateTime.UtcNow;

        if (timeUntilExpiration <= TimeSpan.Zero)
            throw new InvalidOperationException("Cannot revoke an expired token");

        // Store the token in the blacklist with expiration
        var blacklistKey = $"revoked_token:{token}";
        await _cacheService.SetAsync(blacklistKey, true, timeUntilExpiration);
    }

    public async Task<AuthResult> GenerateStreamTokenAsync(Guid streamId)
    {
        // Get the current user's claims
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
            return new AuthResult
            {
                Success = false,
                Error = "User not found"
            };

        // Generate a new stream-specific token
        var token = _tokenService.GenerateStreamToken(currentUser, streamId);

        return new AuthResult
        {
            Success = true,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            UserId = currentUser.Id
        };
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        var userIdClaim = httpContext.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId)) return null;

        var users = await _userRepository.GetAllAsync();
        return users.FirstOrDefault(u => u.Id == userId);
    }
}