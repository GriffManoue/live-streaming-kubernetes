using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseManagementService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseManagementService.Services;

public class PostgreSqlMigrationService : IMigrationService
{
    private readonly MasterDbContext _masterDbContext;
    private readonly ILogger<PostgreSqlMigrationService> _logger;

    public PostgreSqlMigrationService(
        MasterDbContext masterDbContext,
        ILogger<PostgreSqlMigrationService> logger
        )
    {
        _masterDbContext = masterDbContext ?? throw new ArgumentNullException(nameof(masterDbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HasPendingMigrationsAsync()
    {
        return await _masterDbContext.Database.CanConnectAsync();
    }

    public async Task<IEnumerable<string>> ApplyMigrationsAsync()
    {
        await _masterDbContext.Database.MigrateAsync();
        return _masterDbContext.Database.GetPendingMigrations();
    }
}
