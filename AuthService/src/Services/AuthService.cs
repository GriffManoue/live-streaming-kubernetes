using Shared.Interfaces;
using Shared.models.Enums;
using Shared.Models.Auth;
using Shared.Models.Domain;
using StreamService.Services;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly ICacheService _cacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRepository<User> _userRepository;
    private readonly IStreamServiceClient _streamServiceClient;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IRepository<User> userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IHttpContextAccessor httpContextAccessor,
        IStreamServiceClient streamServiceClient,
        ICacheService cacheService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor;
        _streamServiceClient = streamServiceClient;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        
        // Check if user with the same username exists using the repository
        var userWithSameUsername = await _userRepository.FirstOrDefaultAsync(
            u => u.Username.ToLower() == request.Username.ToLower());
            
        if (userWithSameUsername != null)
            return new AuthResult
            {
                Success = false,
                Error = "Username already exists"
            };

        // Check if user with the same email exists
        var userWithSameEmail = await _userRepository.FirstOrDefaultAsync(
            u => u.Email.ToLower() == request.Email.ToLower());
            
        if (userWithSameEmail != null)
            return new AuthResult
            {
                Success = false,
                Error = "Email already exists"
            };

        // Create new user
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = request.Username,
            Email = request.Email,
            Password = _passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsLive = false,
            Followers = new List<User>(),
            Following = new List<User>(),
            CreatedAt = DateTime.UtcNow,
        };

        // Save the user first
        await _userRepository.AddAsync(user);
        
        try
        {
            // Create the stream in StreamService
            var stream = await _streamServiceClient.CreateStreamAsync();
            user.Stream = new LiveStream
            {
                Id = stream.Id,
                StreamName = stream.StreamName,
                StreamDescription = stream.StreamDescription,
                ThumbnailUrl = stream.ThumbnailUrl,
                StreamUrl = stream.StreamUrl,
                StreamCategory = stream.StreamCategory,
                Views = stream.Views
            };
            
            // Update user with stream info
            await _userRepository.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            // Log the error but continue - stream can be created later
            Console.WriteLine($"Failed to create stream for user {userId}: {ex.Message}");
        }

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
        // Find user by username using the enhanced repository
        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.Username.ToLower() == request.Username.ToLower());

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

    private async Task<User?> GetCurrentUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        var userIdClaim = httpContext.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId)) return null;

        // Use the enhanced repository to get the user by ID directly
        return await _userRepository.GetByIdAsync(userId);
    }
}