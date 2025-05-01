using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Add this using directive
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.src.Models.User;

namespace UserDbHandler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserDbHandlerService _userService;
    
    public UserController(IUserDbHandlerService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDTO>> GetUserById(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("username/{username}")]
    public async Task<ActionResult<UserDTO>> GetUserByUsername(string username)
    {
        try
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [Authorize] // Add Authorize attribute
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDTO>> UpdateUser(Guid id, [FromBody] UserDTO userDto)
    {
        try
        {
            // Ensure the ID in the route matches the ID in the DTO
            if (id != userDto.Id)
            {
                return BadRequest("User ID in the URL does not match the ID in the request body");
            }
            
            var updatedUser = await _userService.UpdateUserAsync(userDto);
            return Ok(updatedUser);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [Authorize] // Add Authorize attribute
    [HttpGet("includes/{id:guid}")]
    public async Task<ActionResult<UserDTO>> GetUserByIdWithIncludes(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdWithIncludesAsync(id);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [Authorize] // Add Authorize attribute
    [HttpPost]
    public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserDTO userDto)
    {
        try
        {
            var createdUser = await _userService.CreateUserAsync(userDto);
            return Ok(createdUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }    [Authorize] // Add Authorize attribute
    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [Authorize]
    [HttpPost("follow")]
    public async Task<ActionResult> FollowUser([FromBody] FollowRequest request)
    {
        try
        {
            await _userService.FollowUserAsync(request.FollowerId, request.FollowingId);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [Authorize]
    [HttpPost("unfollow")]
    public async Task<ActionResult> UnfollowUser([FromBody] FollowRequest request)
    {
        try
        {
            await _userService.UnfollowUserAsync(request.FollowerId, request.FollowingId);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
