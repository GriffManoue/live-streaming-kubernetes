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
    
    [HttpPost]
    [AllowAnonymous] // Allow anonymous access for service-to-service communication
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

    [HttpPost("{id:guid}/generateStreamKey")]
    public async Task<ActionResult<string>> GenerateStreamKey(Guid id)
    {
        try
        {
            var streamKey = await _streamService.GenerateStreamKeyAsync(id);
            return Ok(new { streamKey });  // Return as JSON object with property name
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

    [HttpPost("start/{streamKey}")]
    [AllowAnonymous]
    public async Task<ActionResult> StartStream(string streamKey)
    {
        try
        {
            await _streamService.StartStreamAsync(streamKey);
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

    [HttpPost("end/{streamKey}")]
    public async Task<ActionResult> EndStream(string streamKey)
    {
        try
        {
            await _streamService.EndStreamAsync(streamKey);
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

    [HttpPost("{id:guid}/viewer/join")]
    [AllowAnonymous]
    public async Task<IActionResult> JoinViewer(Guid id)
    {
        await _streamService.JoinViewerAsync(id);
        return Ok();
    }

    [HttpPost("{id:guid}/viewer/leave")]
    [AllowAnonymous]
    public async Task<IActionResult> LeaveViewer(Guid id)
    {
        await _streamService.LeaveViewerAsync(id);
        return Ok();
    }

    [HttpGet("{id:guid}/viewers")]
    [AllowAnonymous]
    public async Task<ActionResult<int>> GetViewerCount(Guid id)
    {
        var count = await _streamService.GetViewerCountAsync(id);
        return Ok(count);
    }
}
