using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.Models.Analytics;

namespace AnalyticsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    [HttpGet("stream/{streamId}")]
    public async Task<ActionResult<StreamAnalyticsDto>> GetStreamAnalytics(Guid streamId)
    {
        try
        {
            var analytics = await _analyticsService.GetStreamAnalyticsAsync(streamId);
            return Ok(analytics);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Stream not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stream analytics");
            return StatusCode(500, "An error occurred while retrieving stream analytics");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<UserAnalyticsDto>> GetUserAnalytics(Guid userId)
    {
        try
        {
            var analytics = await _analyticsService.GetUserAnalyticsAsync(userId);
            return Ok(analytics);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user analytics");
            return StatusCode(500, "An error occurred while retrieving user analytics");
        }
    }

    [HttpPost("track/view")]
    public async Task<IActionResult> TrackStreamView([FromBody] StreamViewEvent viewEvent)
    {
        try
        {
            await _analyticsService.TrackStreamViewAsync(
                viewEvent.StreamId,
                viewEvent.UserId,
                viewEvent.Timestamp ?? DateTime.UtcNow);
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking stream view");
            return StatusCode(500, "An error occurred while tracking stream view");
        }
    }

    [HttpPost("track/engagement")]
    public async Task<IActionResult> TrackStreamEngagement([FromBody] StreamEngagementEvent engagementEvent)
    {
        try
        {
            await _analyticsService.TrackStreamEngagementAsync(
                engagementEvent.StreamId,
                engagementEvent.UserId,
                engagementEvent.EngagementType,
                engagementEvent.Timestamp ?? DateTime.UtcNow);
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking stream engagement");
            return StatusCode(500, "An error occurred while tracking stream engagement");
        }
    }
}

public class StreamViewEvent
{
    public Guid StreamId { get; set; }
    public Guid UserId { get; set; }
    public DateTime? Timestamp { get; set; }
}

public class StreamEngagementEvent
{
    public Guid StreamId { get; set; }
    public Guid UserId { get; set; }
    public string EngagementType { get; set; } = string.Empty;
    public DateTime? Timestamp { get; set; }
}
