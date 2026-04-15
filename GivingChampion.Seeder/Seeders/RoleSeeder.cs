using GivingChampion.Domain.Entities;
using GivingChampion.Seeder.Contracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Seeder.Seeders
{
    public class RoleSeeder : Contracts.Seeder
    {
        private readonly IServiceProvider services;
        private readonly ILogger<RoleSeeder> logger;

        public RoleSeeder(IServiceProvider services, ILogger<RoleSeeder> logger) : base(services)
        {
            this.services = services;
            this.logger = logger;
        }

        public override async Task<bool> Seed()
        {
            try
            {
                using var scope = services.CreateScope();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                string[] roles = ["Admin", "User", "Volunteer", "Donor", "Child", "Partner"];

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new ApplicationRole(role));
                        logger.LogInformation($"Role '{role}' created successfully.");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding roles.");
                return false;
            }
            
        }
    }
}
