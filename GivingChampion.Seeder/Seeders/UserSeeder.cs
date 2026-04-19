using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Seeder.Seeders
{
    public class UserSeeder : Contracts.Seeder
    {
        private readonly IServiceProvider services;
        private readonly ILogger<UserSeeder> _logger;
        private readonly IConfiguration _config;

        public UserSeeder(
            IServiceProvider services,
            ILogger<UserSeeder> logger,
            IConfiguration configuration) : base(services)
        {
            _logger = logger;
            _config = configuration;
            this.services = services;
        }

        public override async Task<bool> Seed()
        {
            try
            {
                await using var scope = services.CreateAsyncScope();   // Fresh scope every time

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

                _logger.LogInformation("Starting user seeding...");

                await SeedDefaultAdminUserAsync(userManager, context);

                _logger.LogInformation("User seeding completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding users.");
                return false;
            }
        }

        private async Task SeedDefaultAdminUserAsync(
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            var section = _config.GetSection("DefaultAdminUser");

            if (!section.Exists())
            {
                _logger.LogWarning("DefaultAdminUser configuration section not found. Skipping admin seeding.");
                return;
            }

            var adminConfig = section.Get<DefaultAdminUserConfig>();
            if (adminConfig == null || string.IsNullOrWhiteSpace(adminConfig.Email))
            {
                _logger.LogWarning("Invalid DefaultAdminUser configuration. Skipping admin seeding.");
                return;
            }

            // Check if user already exists
            if (await context.Users.AnyAsync(u => u.Email == adminConfig.Email))
            {
                _logger.LogInformation("Admin user '{Email}' already exists. Skipping creation.", adminConfig.Email);
                return;
            }

            var adminUser = new ApplicationUser
            {
                UserName = adminConfig.Username,
                Email = adminConfig.Email,
                EmailConfirmed = true,
                FullName = adminConfig.FullName,
                BirthDay = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), // Use UTC to avoid Npgsql error
                // Add other properties as needed (e.g. CreatedAt = DateTime.UtcNow)
            };

            // Create the user with password
            var createResult = await userManager.CreateAsync(adminUser, adminConfig.Password);

            if (createResult.Succeeded)
            {
                // Add roles
                if (adminConfig.Roles?.Count > 0)
                {
                    var addRolesResult = await userManager.AddToRolesAsync(adminUser, adminConfig.Roles);

                    if (addRolesResult.Succeeded)
                    {
                        _logger.LogInformation("Successfully seeded admin user '{Email}' with roles: {Roles}",
                            adminConfig.Email, string.Join(", ", adminConfig.Roles));
                    }
                    else
                    {
                        _logger.LogWarning("Failed to assign roles to admin user: {Errors}",
                            string.Join(", ", addRolesResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    _logger.LogInformation("Admin user '{Email}' created successfully (no roles assigned).", adminConfig.Email);
                }
            }
            else
            {
                _logger.LogError("Failed to create admin user '{Email}': {Errors}",
                    adminConfig.Email,
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
    }

    // Simple POCO class to strongly-type your configuration
    public class DefaultAdminUserConfig
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}