using System;
using System.Linq;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models.Auth;
using Shared.Models.Domain;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    
    public AuthService(
        IRepository<User> userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        // Check if user with the same username or email already exists
        var users = await _userRepository.GetAllAsync();
        if (users.Any(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase)))
        {
            return new AuthResult
            {
                Success = false,
                Error = "Username already exists"
            };
        }
        
        if (users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return new AuthResult
            {
                Success = false,
                Error = "Email already exists"
            };
        }
        
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
        {
            return new AuthResult
            {
                Success = false,
                Error = "Invalid username or password"
            };
        }
        
        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.Password))
        {
            return new AuthResult
            {
                Success = false,
                Error = "Invalid username or password"
            };
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
    
    public async Task<AuthResult> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return new AuthResult
            {
                Success = false,
                Error = "Token is required"
            };
        }
        
        if (!_tokenService.ValidateToken(token))
        {
            return new AuthResult
            {
                Success = false,
                Error = "Invalid or expired token"
            };
        }
        
        var principal = _tokenService.GetPrincipalFromToken(token);
        var userIdClaim = principal.FindFirst("sub")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return new AuthResult
            {
                Success = false,
                Error = "Invalid token"
            };
        }
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Error = "User not found"
            };
        }
        
        return new AuthResult
        {
            Success = true,
            Token = token,
            UserId = userId
        };
    }
    
    public async Task RevokeTokenAsync(string token)
    {
        // In a real implementation, you would add the token to a blacklist
        // or invalidate it in some way. For simplicity, we'll just return.
        await Task.CompletedTask;
    }
}
