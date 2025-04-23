using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.Models.Stream;

namespace StreamDbHandler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamDbHandlerController : ControllerBase
{
    private readonly IStreamDbHandlerService _streamService;
    
    public StreamDbHandlerController(IStreamDbHandlerService streamService)
    {
        _streamService = streamService;
    }

    [Authorize]
    [HttpGet("{id:guid}")]
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
    
    [Authorize]
    [HttpGet]
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
    
    [Authorize]
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<StreamDto>> GetStreamByUserId(Guid userId)
    {
        try
        {
            var stream = await _streamService.GetStreamByUserIdAsync(userId);
            if (stream == null)
            {
                return NotFound($"No stream found for user with ID {userId}");
            }
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
    
    [Authorize] // Add Authorize attribute
    [HttpPost]
    public async Task<ActionResult<StreamDto>> CreateStream([FromQuery] Guid? userId = null)
    {
        try
        {
            Console.WriteLine("Creating stream...");
            var stream = await _streamService.CreateStreamAsync(userId);
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
    
    [Authorize] // Add Authorize attribute
    [HttpPut("{id:guid}")]
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

    [Authorize] // Add Authorize attribute
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<StreamDto>>> GetAllStreams()
    {
        try
        {
            var streams = await _streamService.GetAllStreamsAsync();
            return Ok(streams);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
