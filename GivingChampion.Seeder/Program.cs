using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Seeder;
using GivingChampion.Seeder.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// for live
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionstring, b => b.MigrationsAssembly("GivingChampion.Domain")));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<ApplicationRole>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDataProtection();

builder.Services.AddSingleton<RoleSeeder>();

builder.Services.AddHostedService<SeedingWorker>();

var host = builder.Build();
host.Run();
