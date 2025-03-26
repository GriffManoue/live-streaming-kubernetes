using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Interfaces;

namespace StreamService.Controllers;

[ApiController]
[Route("api/rtmp")]
public class RtmpEventController : ControllerBase
{
    private readonly IStreamService _streamService;
    private readonly ILogger<RtmpEventController> _logger;
    
    public RtmpEventController(IStreamService streamService, ILogger<RtmpEventController> logger)
    {
        _streamService = streamService;
        _logger = logger;
    }
    
    [HttpPost("publish")]
    public async Task<ActionResult> OnPublish([FromBody] RtmpPublishEvent @event)
    {
        try
        {
            _logger.LogInformation($"Stream publish event received: {{{@event.StreamKey}, {@event.StreamName}}}");

            await _streamService.StartStreamAsync(@event.StreamKey);
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing stream publish event");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpPost("publish_done")]
    public async Task<ActionResult> OnPublishDone([FromBody] RtmpPublishDoneEvent @event)
    {
        try
        {
            _logger.LogInformation($"Stream publish done event received: {{{@event.StreamKey}, {@event.StreamName}}}");
            
            await _streamService.EndStreamAsync(@event.StreamKey);
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing stream publish done event");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

public class RtmpPublishEvent
{
    public string StreamKey { get; set; } = string.Empty;
    public string StreamName { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientIp { get; set; } = string.Empty;
}

public class RtmpPublishDoneEvent
{
    public string StreamKey { get; set; } = string.Empty;
    public string StreamName { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientIp { get; set; } = string.Empty;
    public int Duration { get; set; }
}
