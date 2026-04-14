using GivingChampion.Domain.Contexts;
using GivingChampion.Seeder.Seeders;

namespace GivingChampion.Seeder;

public class SeedingWorker : BackgroundService
{
    private readonly ILogger<SeedingWorker> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly RoleSeeder _roleSeeder;

    public SeedingWorker(
        ILogger<SeedingWorker> logger,
        IHostApplicationLifetime applicationLifetime,
        RoleSeeder roleSeeder)
    {
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _roleSeeder = roleSeeder;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            bool allSuccessfull = true;
            _logger.LogInformation("Starting database seeding service...");

            var roleSeedingResult = await _roleSeeder.Seed();
            if (!roleSeedingResult)
            {
                allSuccessfull = false;
            }

            if (!allSuccessfull)
            {
                _logger.LogWarning("Warning! Not All Seeders are Successfull.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database seeding failed.");
            throw;
        }
        finally
        {
            _applicationLifetime.StopApplication();
        }
    }
}
