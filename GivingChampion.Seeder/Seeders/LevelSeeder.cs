using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Seeder.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GivingChampion.Seeder.Seeders
{
    public class LevelSeeder : Contracts.Seeder
    {
        private readonly IServiceProvider services;
        private readonly ILogger<LevelSeeder> logger;

        public LevelSeeder(IServiceProvider services, ILogger<LevelSeeder> logger) : base(services)
        {
            this.services = services;
            this.logger = logger;
        }

        public override async Task<bool> Seed()
        {
            try
            {
                using var scope = services.CreateScope();

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var levels = new List<Level>
                {
                    new Level { Number = 1, MaxXp = 100 },
                    new Level { Number = 2, MaxXp = 250 },
                    new Level { Number = 3, MaxXp = 500 },
                    new Level { Number = 4, MaxXp = 900 },
                    new Level { Number = 5, MaxXp = 1400 },
                    new Level { Number = 6, MaxXp = 2000 },
                    new Level { Number = 7, MaxXp = 2800 },
                    new Level { Number = 8, MaxXp = 3800 },
                    new Level { Number = 9, MaxXp = 5000 },
                    new Level { Number = 10, MaxXp = 6500 }
                };

                foreach (var level in levels)
                {
                    var exists = await context.Levels
                        .AnyAsync(l => l.Number == level.Number);

                    if (!exists)
                    {
                        await context.Levels.AddAsync(level);

                        logger.LogInformation(
                            "Level {LevelNumber} created successfully with MaxXp {MaxXp}.",
                            level.Number,
                            level.MaxXp
                        );
                    }
                }

                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding levels.");
                return false;
            }
        }
    }
}