using GivingChampion.Persistence.Contexts;
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
            var existingAdmin = await userManager.FindByEmailAsync(adminConfig.Email);
            if (existingAdmin != null)
            {
                await EnsureAdminRolesAsync(userManager, existingAdmin, adminConfig.Roles);
                await EnsureAdminProfileDataAsync(context, existingAdmin.Id);

                _logger.LogInformation("Admin user '{Email}' already exists. Ensured roles and profile data.", adminConfig.Email);
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

                        await EnsureAdminProfileDataAsync(context, adminUser.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to assign roles to admin user: {Errors}",
                            string.Join(", ", addRolesResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    await EnsureAdminProfileDataAsync(context, adminUser.Id);
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

        private async Task EnsureAdminRolesAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser adminUser,
            List<string> roles)
        {
            if (roles.Count == 0)
                return;

            var currentRoles = await userManager.GetRolesAsync(adminUser);
            var missingRoles = roles
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role.Trim())
                .Distinct()
                .Where(role => !currentRoles.Contains(role))
                .ToList();

            if (missingRoles.Count == 0)
                return;

            var addRolesResult = await userManager.AddToRolesAsync(adminUser, missingRoles);

            if (!addRolesResult.Succeeded)
            {
                _logger.LogWarning("Failed to assign missing roles to admin user: {Errors}",
                    string.Join(", ", addRolesResult.Errors.Select(e => e.Description)));
            }
        }

        private async Task EnsureAdminProfileDataAsync(AppDbContext context, Guid adminUserId)
        {
            if (!await context.Donors.AnyAsync(d => d.UserId == adminUserId && !d.IsDeleted))
            {
                await context.Donors.AddAsync(new Donor { UserId = adminUserId });
            }

            if (!await context.Volunteers.AnyAsync(v => v.UserId == adminUserId && !v.IsDeleted))
            {
                await context.Volunteers.AddAsync(new Volunteer { UserId = adminUserId });
            }

            var profile = await context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == adminUserId && !p.IsDeleted);

            if (profile == null)
            {
                var firstLevel = await context.Levels
                    .OrderBy(level => level.Number)
                    .FirstOrDefaultAsync();

                if (firstLevel == null)
                {
                    _logger.LogWarning("No levels found. Skipping admin profile/avatar seeding.");
                    await context.SaveChangesAsync();
                    return;
                }

                profile = new Profile
                {
                    UserId = adminUserId,
                    LevelId = firstLevel.Id
                };

                await context.Profiles.AddAsync(profile);
                await context.SaveChangesAsync();
            }

            if (!await context.Avatars.AnyAsync(a => a.ProfileId == profile.Id && !a.IsDeleted))
            {
                await context.Avatars.AddAsync(new Avatar { ProfileId = profile.Id });
            }

            await context.SaveChangesAsync();
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
