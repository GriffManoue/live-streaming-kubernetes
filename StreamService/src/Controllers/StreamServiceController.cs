using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.Models.Stream;

namespace StreamService.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamServiceController : ControllerBase
    {
        private readonly IStreamService _streamService;

        public StreamServiceController(IStreamService streamService)
        {
            _streamService = streamService;
        }

        [Authorize]
        [HttpPost("{id:guid}/generateStreamKey")]
        public async Task<ActionResult<string>> GenerateStreamKey(Guid id)
        {
            try
            {
                var streamKey = await _streamService.GenerateStreamKeyAsync(id);
                return Ok(new { streamKey });
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

        [Authorize]
        [HttpGet("{userId:guid}/reccommendations")]
        public async Task<ActionResult<IEnumerable<StreamDto>>> GetRecommendations(Guid userId, [FromQuery] int count = 6)
        {
            try
            {
                var recommendations = await _streamService.GetReccommendedStreamsAsync(userId, count);
                return Ok(recommendations);
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
}
