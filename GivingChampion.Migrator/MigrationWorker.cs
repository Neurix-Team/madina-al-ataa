using GivingChampion.Domain.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;

namespace GivingChampion.Migrator;

public class MigrationWorker : BackgroundService
{
    private readonly ILogger<MigrationWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public MigrationWorker(
        ILogger<MigrationWorker> logger,
        IServiceScopeFactory scopeFactory,
        IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting database migration service...");

            await using var scope = _scopeFactory.CreateAsyncScope();
            var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (_context.Database != null)
            {
                var availableMigrations = await _context.Database.GetPendingMigrationsAsync(stoppingToken);
                if (availableMigrations != null)
                {
                    _logger.LogInformation("Applying EF Core migrations...");
                    await _context.Database.MigrateAsync(stoppingToken);
                    _logger.LogInformation("Database migrations applied successfully.");
                }
                else
                {
                    _logger.LogInformation("No pending migrations found. Database is up to date.");
                }
            }
            else
            {
                _logger.LogWarning("Database connection is not available. Skipping migrations.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database migration failed.");
            throw;
        }
        finally
        {
            _applicationLifetime.StopApplication();
        }
    }
}
