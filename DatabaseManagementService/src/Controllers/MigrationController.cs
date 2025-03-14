using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseManagementService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

    [HttpGet("status")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMigrationStatus()
    {
        try
        {
            var hasPendingMigrations = await _migrationService.HasPendingMigrationsAsync();
            return Ok(new { PendingMigrations = hasPendingMigrations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking migration status");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while checking migration status");
        }
    }

    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailableMigrations()
    {
        try
        {
            var migrations = await _migrationService.GetAvailableMigrationsAsync();
            return Ok(migrations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available migrations");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting available migrations");
        }
    }

    [HttpGet("applied")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAppliedMigrations()
    {
        try
        {
            var migrations = await _migrationService.GetAppliedMigrationsAsync();
            return Ok(migrations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applied migrations");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting applied migrations");
        }
    }

    [HttpPost("migrate")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RunMigration()
    {
        try
        {
            var result = await _migrationService.MigrateAsync();
            if (result)
            {
                return Ok(new { Success = true, Message = "Migration completed successfully" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Migration failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running migration");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while running migration");
        }
    }
}
