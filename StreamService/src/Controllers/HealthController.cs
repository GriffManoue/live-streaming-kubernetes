using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;

namespace StreamService.src.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet("live")]
    public async Task<IActionResult> LivenessCheck()
    {
        var result = await _healthCheckService.CheckHealthAsync();
        return result.Status == HealthStatus.Healthy 
            ? Ok("Healthy") 
            : StatusCode(500, "Unhealthy");
    }

    [HttpGet("ready")]
    public async Task<IActionResult> ReadinessCheck()
    {
        var result = await _healthCheckService.CheckHealthAsync();
        return result.Status == HealthStatus.Healthy 
            ? Ok("Ready") 
            : StatusCode(503, "Not Ready");
    }
}
