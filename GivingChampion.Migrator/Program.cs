using GivingChampion.Domain.Contexts;
using GivingChampion.Migrator;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
var _conf = builder.Configuration;
var _env = builder.Environment;

//var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//// for live
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseNpgsql(connectionstring, b => b.MigrationsAssembly("GivingChampion.Domain")));

if (_env.IsDevelopment())
{
    builder.AddNpgsqlDbContext<AppDbContext>("DefaultConnection");
}
else
{
    var connectionstring = _conf.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionstring, b => b.MigrationsAssembly("GivingChampion.Domain")));
}

builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();
