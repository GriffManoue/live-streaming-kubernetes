using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.Models.Stream;
using Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecommendationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;
    private readonly ILogger<RecommendationController> _logger;

    public RecommendationController(IRecommendationService recommendationService, ILogger<RecommendationController> logger)
    {
        _recommendationService = recommendationService;
        _logger = logger;
    }

    [HttpGet("streams/{userId}")]
    public async Task<ActionResult<IEnumerable<StreamDto>>> GetRecommendedStreams(Guid userId)
    {
        try
        {
            var recommendations = await _recommendationService.GetRecommendedStreamsAsync(userId);
            return Ok(recommendations);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stream recommendations");
            return StatusCode(500, "An error occurred while retrieving stream recommendations");
        }
    }

    [HttpGet("users/{userId}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetRecommendedUsersToFollow(Guid userId)
    {
        try
        {
            var recommendations = await _recommendationService.GetRecommendedUsersToFollowAsync(userId);
            return Ok(recommendations);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user recommendations");
            return StatusCode(500, "An error occurred while retrieving user recommendations");
        }
    }
}
