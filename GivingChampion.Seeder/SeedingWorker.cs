using GivingChampion.Domain.Contexts;
using GivingChampion.Seeder.Seeders;

namespace GivingChampion.Seeder;

public class SeedingWorker : BackgroundService
{
    private readonly ILogger<SeedingWorker> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly RoleSeeder _roleSeeder;
    private readonly UserSeeder _userSeeder;
    private readonly LevelSeeder _levelSeeder;

    public SeedingWorker(
        ILogger<SeedingWorker> logger,
        IHostApplicationLifetime applicationLifetime,
        RoleSeeder roleSeeder,
        UserSeeder userSeeder,
        LevelSeeder levelSeeder)
    {
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _roleSeeder = roleSeeder;
        _userSeeder = userSeeder;
        _levelSeeder = levelSeeder;
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

            var userSeedingResult = await _userSeeder.Seed();
            if (!userSeedingResult)
            {
                allSuccessfull = false;
            }

            var levelSeedingResult = await _levelSeeder.Seed();
            if (!levelSeedingResult)
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
