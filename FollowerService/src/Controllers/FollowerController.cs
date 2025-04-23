using Microsoft.AspNetCore.Mvc;
using Shared.src.Interfaces.Services;
using Shared.src.Models.User;

namespace FollowerService.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowerController : ControllerBase
    {
        private readonly IFollowerService _followerService;

        public FollowerController(IFollowerService followerService)
        {
            _followerService = followerService;
        }

        [HttpGet("{userId:guid}/followers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetFollowers(Guid userId)
        {
            try
            {
                var followers = await _followerService.GetFollowersAsync(userId);
                return Ok(followers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{userId:guid}/following")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetFollowing(Guid userId)
        {
            try
            {
                var following = await _followerService.GetFollowingAsync(userId);
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
                await _followerService.FollowUserAsync(request.FollowerId, request.FollowingId);
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
                await _followerService.UnfollowUserAsync(request.FollowerId, request.FollowingId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
