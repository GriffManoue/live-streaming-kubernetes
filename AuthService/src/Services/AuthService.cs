using Shared.Interfaces;
using Shared.models.Enums;
using Shared.Models.Auth;
using Shared.Models.Domain;
using StreamDbHandler.Services;
using System.Linq;
using Microsoft.Extensions.Logging;
using Shared.Interfaces.Clients;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly ICacheService _cacheService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IStreamDbHandlerClient _streamServiceClient;
    private readonly ILogger<AuthService> _logger;
    private readonly IUserDbHandlerClient _userDbHandlerClient;

    public AuthService(
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IStreamDbHandlerClient streamServiceClient,
        ICacheService cacheService,
        ILogger<AuthService> logger,
        IUserDbHandlerClient userDbHandlerClient)
    {
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _streamServiceClient = streamServiceClient;
        _cacheService = cacheService;
        _logger = logger;
        _userDbHandlerClient = userDbHandlerClient;
    }

    private User MapToUser(UserDTO dto)
    {
        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Password = dto.Password,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CreatedAt = dto.CreatedAt,
            IsLive = dto.IsLive,
            Followers = new List<User>(),
            Following = new List<User>()
        };
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        // Check if user with the same username exists using IUserDbHandlerClient
        UserDTO? userWithSameUsername = null;
        try
        {
            userWithSameUsername = await _userDbHandlerClient.GetUserByUsernameAsync(request.Username);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // User not found, this is expected for registration
            userWithSameUsername = null;
        }
        catch (KeyNotFoundException)
        {
            // User not found, this is expected for registration
            userWithSameUsername = null;
        }
        if (userWithSameUsername != null)
            return new AuthResult
            {
                Success = false,
                Error = "Username already exists"
            };

        // Check if user with the same email exists
        UserDTO? userWithSameEmail = null;
        try
        {
            userWithSameEmail = await _userDbHandlerClient.GetUserByEmailAsync(request.Email);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            userWithSameEmail = null;
        }
        catch (KeyNotFoundException)
        {
            userWithSameEmail = null;
        }
        if (userWithSameEmail != null)
            return new AuthResult
            {
                Success = false,
                Error = "Email already exists"
            };

        // Create new user
        var userDto = new UserDTO
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsLive = false,
            CreatedAt = DateTime.UtcNow,
            FollowerIds = new List<Guid>(),
            FollowingIds = new List<Guid>()
        };
        var createdUser = await _userDbHandlerClient.CreateUserAsync(userDto);
        try
        {
            // Create the stream in StreamService - pass the user object so the StreamService can use its ID
            var stream = await _streamServiceClient.CreateStreamAsync(createdUser.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create stream for user {UserId}: {Message}", createdUser.Id, ex.Message);
          
        }
        // Generate token
        var token = _tokenService.GenerateToken(MapToUser(createdUser));
        return new AuthResult
        {
            Success = true,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            UserId = createdUser.Id
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        // Find user by username using IUserDbHandlerClient
        UserDTO? user = null;
        try
        {
            user = await _userDbHandlerClient.GetUserByUsernameAsync(request.Username);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // User not found
            user = null;
        }
        catch (KeyNotFoundException)
        {
            user = null;
        }
        if (user == null)
            return new AuthResult
            {
                Success = false,
                Error = "Invalid username"
            };
        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.Password))
            return new AuthResult
            {
                Success = false,
                Error = "Invalid password"
            };
        // Generate token
        var token = _tokenService.GenerateToken(MapToUser(user));
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
        var user = await _userDbHandlerClient.GetUserByIdAsync(userId);
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
}