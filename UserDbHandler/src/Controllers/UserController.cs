using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;

namespace UserDbHandler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
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
    
    [HttpGet("{id:guid}/followers")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetFollowers(Guid id)
    {
        try
        {
            var followers = await _userService.GetFollowersAsync(id);
            return Ok(followers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("{id:guid}/following")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetFollowing(Guid id)
    {
        try
        {
            var following = await _userService.GetFollowingAsync(id);
            return Ok(following);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
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
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpPost("unfollow")]
    public async Task<ActionResult> UnfollowUser([FromBody] FollowRequest request)
    {
        try
        {
            await _userService.UnfollowUserAsync(request.FollowerId, request.FollowingId);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

public class FollowRequest
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}
