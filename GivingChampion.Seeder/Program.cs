using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Seeder;
using GivingChampion.Seeder.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Bind the DefaultAdminUser section (optional but recommended)
builder.Services.Configure<DefaultAdminUserConfig>(
    builder.Configuration.GetSection("DefaultAdminUser"));

builder.AddServiceDefaults();
var _conf = builder.Configuration;
var _env = builder.Environment;

if (_env.IsDevelopment())
{
    // for testing
    builder.AddNpgsqlDbContext<AppDbContext>("givingchampion");
}
else
{
    var connectionstring = _conf.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    // for live
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionstring, b => b.MigrationsAssembly("GivingChampion.Domain")));
}

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDataProtection();

builder.Services.AddSingleton<RoleSeeder>();
builder.Services.AddSingleton<UserSeeder>();

builder.Services.AddHostedService<SeedingWorker>();

var host = builder.Build();
host.Run();
