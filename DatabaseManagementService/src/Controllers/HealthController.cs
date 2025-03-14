using Microsoft.AspNetCore.Mvc;

namespace DatabaseManagementService.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Status = "Healthy", Service = "Database Management Service" });
    }
}
