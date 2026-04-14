using GivingChampion.Domain.Contexts;
using GivingChampion.Migrator;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// for live
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionstring, b => b.MigrationsAssembly("GivingChampion.Domain")));

builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();
