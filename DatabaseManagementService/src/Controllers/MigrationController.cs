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
    public async Task<IActionResult> GetMigrationStatus([FromQuery] string serviceName = null)
    {
        try
        {
            var hasPendingMigrations = await _migrationService.HasPendingMigrationsAsync(serviceName);
            return Ok(new { PendingMigrations = hasPendingMigrations, ServiceName = serviceName ?? "Default" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking migration status {(serviceName != null ? $"for {serviceName}" : "")}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while checking migration status {(serviceName != null ? $"for {serviceName}" : "")}");
        }
    }

    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailableMigrations([FromQuery] string serviceName = null)
    {
        try
        {
            var migrations = await _migrationService.GetAvailableMigrationsAsync(serviceName);
            return Ok(migrations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting available migrations {(serviceName != null ? $"for {serviceName}" : "")}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting available migrations {(serviceName != null ? $"for {serviceName}" : "")}");
        }
    }

    [HttpGet("applied")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAppliedMigrations([FromQuery] string serviceName = null)
    {
        try
        {
            var migrations = await _migrationService.GetAppliedMigrationsAsync(serviceName);
            return Ok(migrations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting applied migrations {(serviceName != null ? $"for {serviceName}" : "")}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while getting applied migrations {(serviceName != null ? $"for {serviceName}" : "")}");
        }
    }

    [HttpPost("migrate")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RunMigration([FromBody] string serviceName = null)
    {
        try
        {
            _logger.LogInformation($"Starting migration {(serviceName != null ? $"for {serviceName}" : "for default service")}");
            
            var result = await _migrationService.MigrateAsync(serviceName);
            if (result)
            {
                return Ok(new { Success = true, Message = $"Migration completed successfully {(serviceName != null ? $"for {serviceName}" : "")}" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Migration failed {(serviceName != null ? $"for {serviceName}" : "")}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error running migration {(serviceName != null ? $"for {serviceName}" : "")}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while running migration {(serviceName != null ? $"for {serviceName}" : "")}");
        }
    }
}
