using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.Models.Auth;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResult
            {
                Success = false,
                Error = $"Internal server error: {ex.Message}"
            });
        }
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResult
            {
                Success = false,
                Error = $"Internal server error: {ex.Message}"
            });
        }
    }
    
    [HttpPost("validate")]
    public async Task<ActionResult<AuthResult>> ValidateToken([FromBody] ValidateTokenRequest request)
    {
        try
        {
            var result = await _authService.ValidateTokenAsync(request.Token);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResult
            {
                Success = false,
                Error = $"Internal server error: {ex.Message}"
            });
        }
    }
    
    [HttpPost("revoke")]
    public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        try
        {
            await _authService.RevokeTokenAsync(request.Token);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Error = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpPost("stream-token")]
    [Authorize] 
    public async Task<ActionResult<AuthResult>> GenerateStreamToken([FromBody] StreamTokenRequest request)
    {
        try
        {
            var result = await _authService.GenerateStreamTokenAsync(request.StreamId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResult
            {
                Success = false,
                Error = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpPost("validate-stream-token")]
    [Authorize]
    public async Task<ActionResult<AuthResult>> ValidateStreamToken([FromBody] ValidateTokenRequest request)
    {
        try
        {
            var result = await _authService.ValidateStreamTokenAsync(request.Token);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResult
            {
                Success = false,
                Error = $"Internal server error: {ex.Message}"
            });
        }
    }
}

public class ValidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
}

public class RevokeTokenRequest
{
    public string Token { get; set; } = string.Empty;
}

public class StreamTokenRequest
{
    public Guid StreamId { get; set; }
}
