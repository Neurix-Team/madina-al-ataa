using GivingChampion.API.Repositories;
using GivingChampion.API.Tests.Infrastructure;
using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.DTO.Notification;
using GivingChampion.Application.DTO.ServiceRequestDto;
using GivingChampion.Application.DTO.VolunteerOrder;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Reward;
using GivingChampion.Application.Services;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistence.Contexts;
using GivingChampion.Persistance.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ActivityHistoryDto = GivingChampion.Application.DTO.GivingChampion.Application.DTO.ActivityDto.ActivityDto;

namespace GivingChampion.API.Tests.Integration;

[Collection(DatabaseCollection.Name)]
public class ServiceRequestVolunteerOrderIntegrationTests
{
    private readonly DatabaseTestFixture _fixture;

    public ServiceRequestVolunteerOrderIntegrationTests(DatabaseTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ServiceRequestService_CreateAsync_PersistsApprovedRequest()
    {
        if (DatabaseUnavailable())
            return;

        await using var context = _fixture.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        var actor = CreateUser();
        var level = CreateLevel(1);
        var location = CreateLocation();
        var partner = CreatePartner();

        context.Users.Add(actor);
        context.Levels.Add(level);
        context.Locations.Add(location);
        context.Partners.Add(partner);
        await context.SaveChangesAsync();

        var service = CreateServiceRequestService(context, actor.Id);
        var request = new CreateServiceRequestDto
        {
            Title = "Integration teaching request",
            ServiceType = "Education",
            RequiredSkill = "Teaching",
            UrgencyLevel = UrgencyLevel.Medium,
            ScheduleDate = DateTime.UtcNow.AddDays(3),
            Duration = 4,
            BriefDescription = "Integration test request",
            PartnerId = partner.Id,
            LocationId = location.Id,
            MaxOrders = 2,
            RequiredLevelId = level.Id
        };

        var created = await service.CreateAsync(request);

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Approved", created.Status);
        Assert.Equal(partner.OrgName, created.FullName);

        var persisted = await context.ServiceRequests
            .AsNoTracking()
            .SingleAsync(sr => sr.Id == created.Id);

        Assert.Equal(RequestStatus.Approved, persisted.Status);
        Assert.Equal(request.Title, persisted.Title);
        Assert.Equal(request.RequiredLevelId, persisted.RequiredLevelId);
        Assert.Equal(request.MaxOrders, persisted.MaxOrders);

        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task VolunteerOrderService_CreateAsync_PersistsPendingOrderForEligibleVolunteer()
    {
        if (DatabaseUnavailable())
            return;

        await using var context = _fixture.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        var seed = await SeedVolunteerOrderScenarioAsync(context);
        var notificationService = new FakeNotificationService();
        var rewardService = new FakeRewardSystemService();
        var service = CreateVolunteerOrderService(
            context,
            seed.VolunteerUser.Id,
            notificationService,
            rewardService);

        var created = await service.CreateAsync(new CreateVolunteerOrderDto
        {
            ServiceRequestId = seed.ServiceRequest.Id
        });

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(seed.ServiceRequest.Id, created.ServiceRequestId);
        Assert.Equal("Education", created.ServiceType);

        var persisted = await context.VolunteerOrders
            .AsNoTracking()
            .SingleAsync(order => order.Id == created.Id);

        Assert.Equal(seed.VolunteerUser.Id, persisted.UserId);
        Assert.Equal(OrderStatus.Pending, persisted.Status);
        Assert.False(persisted.IsDeleted);

        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task VolunteerOrderService_ApproveOrderAsync_UpdatesOrderAndServiceRequest()
    {
        if (DatabaseUnavailable())
            return;

        await using var context = _fixture.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        var seed = await SeedVolunteerOrderScenarioAsync(context);
        var order = new VolunteerOrder
        {
            Id = Guid.NewGuid(),
            ServiceRequestId = seed.ServiceRequest.Id,
            UserId = seed.VolunteerUser.Id,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        context.VolunteerOrders.Add(order);
        await context.SaveChangesAsync();

        var notificationService = new FakeNotificationService();
        var rewardService = new FakeRewardSystemService();
        var service = CreateVolunteerOrderService(
            context,
            seed.AdminUser.Id,
            notificationService,
            rewardService);

        var approved = await service.ApproveOrderAsync(order.Id);

        Assert.NotNull(approved);
        Assert.Equal("Approved", approved!.Status);

        var persistedOrder = await context.VolunteerOrders
            .AsNoTracking()
            .SingleAsync(vo => vo.Id == order.Id);
        var persistedRequest = await context.ServiceRequests
            .AsNoTracking()
            .SingleAsync(sr => sr.Id == seed.ServiceRequest.Id);

        Assert.Equal(OrderStatus.Approved, persistedOrder.Status);
        Assert.Equal(RequestStatus.Assigned, persistedRequest.Status);
        Assert.Single(notificationService.CreatedNotifications);

        await transaction.RollbackAsync();
    }

    private ServiceRequestService CreateServiceRequestService(AppDbContext context, Guid userId)
    {
        return new ServiceRequestService(
            new UnitOfWork(context),
            _fixture.Mapper,
            new FakeActivityService(),
            new LevelRepository(context),
            new ServiceRequestRepository(context),
            new GenericRepository<VolunteerOrder>(context),
            CreateHttpContextAccessor(userId));
    }

    private bool DatabaseUnavailable() => !_fixture.IsDatabaseAvailable;

    private VolunteerOrderService CreateVolunteerOrderService(
        AppDbContext context,
        Guid userId,
        FakeNotificationService notificationService,
        FakeRewardSystemService rewardService)
    {
        return new VolunteerOrderService(
            new VolunteerOrderRepository(context),
            new GenericRepository<VolunteerOrder>(context),
            new ServiceRequestRepository(context),
            notificationService,
            rewardService,
            CreateHttpContextAccessor(userId),
            new FakeActivityService(),
            new UnitOfWork(context),
            _fixture.Mapper);
    }

    private async Task<VolunteerOrderScenario> SeedVolunteerOrderScenarioAsync(AppDbContext context)
    {
        var adminUser = CreateUser();
        var volunteerUser = CreateUser();
        var level = CreateLevel(3);
        var location = CreateLocation();
        var partner = CreatePartner();
        var profile = new Profile
        {
            Id = Guid.NewGuid(),
            UserId = volunteerUser.Id,
            LevelId = level.Id,
            Rating = 0,
            Impact = 0
        };
        var userLevel = new UserLevel
        {
            Id = Guid.NewGuid(),
            ProfileId = profile.Id,
            LevelId = level.Id,
            Xp = 0,
            Kp = 0
        };
        var serviceRequest = new ServiceRequest
        {
            Id = Guid.NewGuid(),
            Title = "Integration order request",
            ServiceType = "Education",
            RequiredSkill = "Teaching",
            UrgencyLevel = UrgencyLevel.Medium,
            ScheduleDate = DateTime.UtcNow.AddDays(5),
            LocationId = location.Id,
            Duration = 3,
            MaxOrders = 2,
            RequiredLevelId = level.Id,
            Status = RequestStatus.Approved,
            BriefDescription = "Integration volunteer order request",
            PartnerId = partner.Id,
            Progress = 0,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(adminUser, volunteerUser);
        context.Levels.Add(level);
        context.Locations.Add(location);
        context.Partners.Add(partner);
        context.Profiles.Add(profile);
        context.UserLevels.Add(userLevel);
        context.ServiceRequests.Add(serviceRequest);
        await context.SaveChangesAsync();

        return new VolunteerOrderScenario(adminUser, volunteerUser, serviceRequest);
    }

    private static ApplicationUser CreateUser()
    {
        var id = Guid.NewGuid();
        var email = $"integration-{id:N}@example.test";

        return new ApplicationUser
        {
            Id = id,
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("N"),
            FullName = "Integration User",
            BirthDay = DateTime.UtcNow.AddYears(-25),
            City = "Cairo",
            Address = "Integration address"
        };
    }

    private static Level CreateLevel(int number)
        => new()
        {
            Id = Guid.NewGuid(),
            Number = number,
            MaxXp = number * 100,
            CreatedAt = DateTime.UtcNow
        };

    private static Location CreateLocation()
        => new()
        {
            Id = Guid.NewGuid(),
            Name = "Integration location",
            RequiredLevel = 1,
            Latitude = "30.0444",
            Longitude = "31.2357",
            CreatedAt = DateTime.UtcNow
        };

    private static Partner CreatePartner()
        => new()
        {
            Id = Guid.NewGuid(),
            OrgName = "Integration Partner",
            OrgType = OrgType.Foundation,
            Verified = true,
            CreatedAt = DateTime.UtcNow
        };

    private static IHttpContextAccessor CreateHttpContextAccessor(Guid userId)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
                    "IntegrationTest"))
        };

        return new HttpContextAccessor { HttpContext = httpContext };
    }

    private sealed record VolunteerOrderScenario(
        ApplicationUser AdminUser,
        ApplicationUser VolunteerUser,
        ServiceRequest ServiceRequest);

    private sealed class FakeActivityService : IActivityService
    {
        public List<CreateActivityDto> CreatedActivities { get; } = [];

        public Task<Result<PagedList<ActivityHistoryDto>>> GetEntityHistory(
            Guid entityId,
            PageParameters pageParameters)
            => throw new NotSupportedException();

        public Task AddAsync(CreateActivityDto createActivityDto)
        {
            CreatedActivities.Add(createActivityDto);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeNotificationService : INotificationService
    {
        public List<CreateNotificationDto> CreatedNotifications { get; } = [];

        public Task<Result<PagedList<NotificationDto>>> GetMyNotificationsAsync(
            bool unreadOnly = false,
            PageParameters pageParameters = null!)
            => throw new NotSupportedException();

        public Task<Result> MarkAsReadAsync(Guid notificationId)
            => throw new NotSupportedException();

        public Task<Result> MarkAllAsReadAsync()
            => throw new NotSupportedException();

        public Task CreateNotificationAsync(CreateNotificationDto notification)
        {
            CreatedNotifications.Add(notification);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRewardSystemService : IRewardSystemService
    {
        public List<Guid> RewardedVolunteerOrderIds { get; } = [];
        public List<Guid> RewardedServiceRequestIds { get; } = [];

        public Task RewardServiceRequestCompletedAsync(
            Guid serviceRequestId,
            CancellationToken cancellationToken = default)
        {
            RewardedServiceRequestIds.Add(serviceRequestId);
            return Task.CompletedTask;
        }

        public Task RewardVolunteerOrderCompletedAsync(
            Guid volunteerOrderId,
            CancellationToken cancellationToken = default)
        {
            RewardedVolunteerOrderIds.Add(volunteerOrderId);
            return Task.CompletedTask;
        }

        public Task RewardDonationOrderCompletedAsync(
            Guid donationOrderId,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task RewardUserMissionCompletedAsync(
            Guid userMissionId,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task RewardUserGeoQuestCompletedAsync(
            Guid userGeoQuestId,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
