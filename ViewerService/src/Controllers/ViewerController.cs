using Microsoft.AspNetCore.Authorization; // Add this using directive
using Microsoft.AspNetCore.Mvc;
using Shared.src.Interfaces.Services;

namespace ViewerService.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewerController : ControllerBase
    {
        private readonly IViewerService _viewerService;

        public ViewerController(IViewerService viewerService)
        {
            _viewerService = viewerService;
        }

        [Authorize]
        [HttpGet("{streamId:guid}/count")]
        public async Task<IActionResult> GetViewerCount(Guid streamId)
        {
            var count = await _viewerService.GetViewerCountAsync(streamId);
            return Ok(count);
        }

        [Authorize] // Add Authorize attribute
        [HttpPost("{streamId:guid}/join")]
        public async Task<IActionResult> JoinViewer(Guid streamId, [FromQuery] string viewerId)
        {
            await _viewerService.JoinViewerAsync(streamId, viewerId);
            return Ok();
        }

        [Authorize] // Add Authorize attribute
        [HttpPost("{streamId:guid}/leave")]
        public async Task<IActionResult> LeaveViewer(Guid streamId, [FromQuery] string viewerId)
        {
            await _viewerService.LeaveViewerAsync(streamId, viewerId);
            return Ok();
        }
    }
}
