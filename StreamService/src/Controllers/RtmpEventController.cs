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
    public async Task<ActionResult> OnPublish()
    {
        try
        {
            // Read stream key from form data (NGINX RTMP sends as form data, not JSON)
            var name = Request.Form["name"].ToString(); // This is your stream key
            var app = Request.Form["app"].ToString();   // This is usually "live"
            var clientId = Request.Form["addr"].ToString() ?? string.Empty;
            var clientIp = Request.Form["clientid"].ToString() ?? string.Empty;
            
            _logger.LogInformation($"Stream publish event received: {{Stream Key: {name}, App: {app}}}");

            await _streamService.StartStreamAsync(name);
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing stream publish event");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpPost("publish_done")]
    public async Task<ActionResult> OnPublishDone()
    {
        try
        {
            // Read stream key from form data (NGINX RTMP sends as form data, not JSON)
            var name = Request.Form["name"].ToString(); // This is your stream key
            var app = Request.Form["app"].ToString();   // This is usually "live"
            var clientId = Request.Form["addr"].ToString() ?? string.Empty;
            var clientIp = Request.Form["clientid"].ToString() ?? string.Empty;
            
            _logger.LogInformation($"Stream publish done event received: {{Stream Key: {name}, App: {app}}}");
            
            await _streamService.EndStreamAsync(name);
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing stream publish done event");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
