using DatabaseManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseManagementService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MigrationController : ControllerBase
{
    private readonly IMigrationService _migrationService;
    private readonly ILogger<MigrationController> _logger;

    public MigrationController(IMigrationService migrationService, ILogger<MigrationController> logger)
    {
        _migrationService = migrationService ?? throw new ArgumentNullException(nameof(migrationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    
    [HttpPost("migrate")]
    public async Task<IActionResult> Migrate()
    {
        try
        {
            var appliedMigrations = await _migrationService.ApplyMigrationsAsync();
            return Ok(new { AppliedMigrations = appliedMigrations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying migrations");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Failed to apply migrations", Message = ex.Message });
        }
    }

    [HttpGet("pending")]
    public async Task<IActionResult> Pending()
    {
        try
        {
            var hasPendingMigrations = await _migrationService.HasPendingMigrationsAsync();
            return Ok(new { HasPendingMigrations = hasPendingMigrations }); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for pending migrations");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Failed to check pending migrations", Message = ex.Message });
        }
    }
}
