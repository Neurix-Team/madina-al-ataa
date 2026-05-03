using GivingChampion.Persistence.Contexts;
using GivingChampion.Migrator;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
var _conf = builder.Configuration;
var _env = builder.Environment;

//var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//// for live
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseNpgsql(connectionstring, b => b.MigrationsAssembly("GivingChampion.Domain")));

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

if (_env.IsDevelopment())
{
    builder.AddNpgsqlDbContext<AppDbContext>(
        "DefaultConnection",
        configureDbContextOptions: options =>
        {
            options.UseNpgsql(npgsql =>
                npgsql.MigrationsAssembly("GivingChampion.Persistance"));
        });
}
else
{
    var connectionString = _conf.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString, npgsql =>
            npgsql.MigrationsAssembly("GivingChampion.Persistance")));
}

builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();
