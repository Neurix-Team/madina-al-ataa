using AutoMapper;
using GivingChampion.API.Controllers;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Mapper;
using GivingChampion.Application.Services;
using GivingChampion.Persistence.Contexts;
using GivingChampion.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace GivingChampion.API.Tests.Infrastructure;

public sealed class DatabaseTestFixture : IAsyncLifetime
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public DatabaseTestFixture()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Test.json", optional: false)
            .AddEnvironmentVariables("GivingChampionTests_")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly("GivingChampion.Domain"))
            .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        Mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance).CreateMapper();
    }

    public IMapper Mapper { get; }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_dbContextOptions);
    }

    public DonationRequestsController CreateDonationRequestsController(Guid? userId = null, params string[] roles)
    {
        var context = CreateDbContext();

        var controllerContext = TestAuthContextFactory.CreateControllerContext(userId, roles);
        var httpContextAccessor = TestAuthContextFactory.CreateHttpContextAccessor(userId, roles);

        IDonationRequestService service = new DonationRequestService(
            new DonationRequestRepository(context),
            new ActivityService(
                new ActivityRepository(context),
                new UnitOfWork(context),
                Mapper,
                httpContextAccessor),
            new UnitOfWork(context),
            Mapper,
            httpContextAccessor);

        return new DonationRequestsController(service)
        {
            ControllerContext = controllerContext
        };
    }
    public async Task InitializeAsync()
    {
        await using var context = CreateDbContext();
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
