using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.Models.Stream;

namespace StreamService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamController : ControllerBase
{
    private readonly IStreamService _streamService;
    
    public StreamController(IStreamService streamService)
    {
        _streamService = streamService;
    }
    
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<StreamDto>> GetStreamById(Guid id)
    {
        try
        {
            var stream = await _streamService.GetStreamByIdAsync(id);
            return Ok(stream);
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
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<StreamDto>>> GetActiveStreams()
    {
        try
        {
            var streams = await _streamService.GetActiveStreamsAsync();
            return Ok(streams);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpGet("user/{userId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<StreamDto>>> GetStreamsByUserId(Guid userId)
    {
        try
        {
            var streams = await _streamService.GetStreamsByUserIdAsync(userId);
            return Ok(streams);
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
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<StreamDto>> CreateStream()
    {
        try
        {
            var stream = await _streamService.CreateStreamAsync();
            return CreatedAtAction(nameof(GetStreamById), new { id = stream.Id }, stream);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<StreamDto>> UpdateStream(Guid id, [FromBody] StreamDto streamDto)
    {
        try
        {
            // Ensure the ID in the route matches the ID in the DTO
            if (id != streamDto.Id)
            {
                return BadRequest("Stream ID in the URL does not match the ID in the request body");
            }
            
            var stream = await _streamService.UpdateStreamAsync(id, streamDto);
            return Ok(stream);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpPost("{id:guid}/end")]
    [Authorize]
    public async Task<ActionResult> EndStream(Guid id)
    {
        try
        {
            await _streamService.EndStreamAsync(id);
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
}
