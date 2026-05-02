using GivingChampion.API.Controllers;
using GivingChampion.API.Tests.Infrastructure;
using GivingChampion.Common.DTO.DonationRequest;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Tests.Controllers;

[Collection(DatabaseCollection.Name)]
public class DonationRequestsControllerTests
{
    private readonly DatabaseTestFixture _fixture;

    public DonationRequestsControllerTests(DatabaseTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAll_ReturnsPagedDonationRequestsFromDatabase()
    {
        var seed = await SeedDonationRequestAsync(RequestStatus.Pending);
        var donationRequestId = seed.DonationRequestId!.Value;
        var controller = _fixture.CreateDonationRequestsController(Guid.NewGuid(), "Admin");

        try
        {
            var response = await controller.GetAll(new PageParameters { PageNumber = 1, PageSize = 100 });

            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var page = Assert.IsType<PagedList<DonationRequestDto>>(okResult.Value);
            Assert.Contains(page.Items, item => item.Id == donationRequestId);
        }
        finally
        {
            await DeleteDonationSeedAsync(seed);
        }
    }

    [Fact]
    public async Task GetMyDonationRequests_UsesCurrentUserAndReturnsRequestsBackedByDatabase()
    {
        var donorId = Guid.NewGuid();
        var seed = await SeedDonationRequestAsync(RequestStatus.Approved, donorId: donorId);
        var donationRequestId = seed.DonationRequestId!.Value;
        var controller = _fixture.CreateDonationRequestsController(donorId, "Parent");

        try
        {
            var response = await controller.GetMyDonationRequests(new PageParameters { PageNumber = 1, PageSize = 20 });

            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var page = Assert.IsType<PagedList<DonationRequestDto>>(okResult.Value);
            Assert.Contains(page.Items, item => item.Id == donationRequestId);
        }
        finally
        {
            await DeleteDonationSeedAsync(seed);
        }
    }

    [Fact]
    public async Task GetById_WhenAdmin_ReturnsAnyDonationRequestFromDatabase()
    {
        var adminId = Guid.NewGuid();
        var seed = await SeedDonationRequestAsync(RequestStatus.Pending);
        var donationRequestId = seed.DonationRequestId!.Value;
        var controller = _fixture.CreateDonationRequestsController(adminId, "Admin");

        try
        {
            var response = await controller.GetById(donationRequestId);

            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var dto = Assert.IsType<DonationRequestDto>(okResult.Value);
            Assert.Equal(donationRequestId, dto.Id);
            Assert.Equal(seed.Title, dto.Title);
        }
        finally
        {
            await DeleteDonationSeedAsync(seed);
        }
    }

    [Fact]
    public async Task Create_PersistsDonationRequestAndReturnsCreatedAtAction()
    {
        var parentId = Guid.NewGuid();
        var seed = await SeedDonationDependenciesAsync();
        var controller = _fixture.CreateDonationRequestsController(parentId, "Parent");
        var request = new CreateDonationRequestDto
        {
            Title = $"Test create {Guid.NewGuid():N}",
            LocationId = seed.LocationId,
            DonateAmount = 250,
            UrgencyLevel = UrgencyLevel.Medium,
            BriefDescription = "Created by database-backed API test",
            PartnerId = seed.PartnerId
        };
        Guid? createdDonationRequestId = null;

        try
        {
            var response = await controller.Create(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(response.Result);
            Assert.Equal(nameof(DonationRequestsController.GetById), createdResult.ActionName);
            var dto = Assert.IsType<DonationRequestDto>(createdResult.Value);
            createdDonationRequestId = dto.Id;
            Assert.Equal(request.Title, dto.Title);
            Assert.Equal(RequestStatus.Pending.ToString(), dto.Status);

            await using var context = _fixture.CreateDbContext();
            Assert.True(await context.DonationRequests.AnyAsync(dr => dr.Id == dto.Id));
        }
        finally
        {
            await DeleteDonationSeedAsync(seed with { DonationRequestId = createdDonationRequestId });
        }
    }

    [Fact]
    public async Task Update_PersistsChangesAndReturnsNoContent()
    {
        var seed = await SeedDonationRequestAsync(RequestStatus.Pending);
        var controller = _fixture.CreateDonationRequestsController(Guid.NewGuid(), "Admin");
        var update = new UpdateDonationRequestDto
        {
            Title = $"Updated {Guid.NewGuid():N}",
            DonateAmount = 500
        };

        try
        {
            var response = await controller.Update(seed.DonationRequestId!.Value, update);

            Assert.IsType<NoContentResult>(response);

            await using var context = _fixture.CreateDbContext();
            var entity = await context.DonationRequests.AsNoTracking().SingleAsync(dr => dr.Id == seed.DonationRequestId);
            Assert.Equal(update.Title, entity.Title);
            Assert.Equal(update.DonateAmount, entity.DonateAmount);
        }
        finally
        {
            await DeleteDonationSeedAsync(seed);
        }
    }

    [Fact]
    public async Task Delete_SoftDeletesDonationRequestAndReturnsNoContent()
    {
        var seed = await SeedDonationRequestAsync(RequestStatus.Pending);
        var controller = _fixture.CreateDonationRequestsController(Guid.NewGuid(), "Parent");

        try
        {
            var response = await controller.Delete(seed.DonationRequestId!.Value);

            Assert.IsType<NoContentResult>(response);

            await using var context = _fixture.CreateDbContext();
            var exists = await context.DonationRequests
                .IgnoreQueryFilters()
                .AsNoTracking()
                .AnyAsync(dr => dr.Id == seed.DonationRequestId);
            Assert.False(exists);
        }
        finally
        {
            await DeleteDonationSeedAsync(seed);
        }
    }

    [Fact]
    public async Task Approve_PersistsApprovedStatusAndReturnsNoContent()
    {
        var seed = await SeedDonationRequestAsync(RequestStatus.Pending);
        var controller = _fixture.CreateDonationRequestsController(Guid.NewGuid(), "Admin");

        try
        {
            var response = await controller.Approve(seed.DonationRequestId!.Value);

            Assert.IsType<NoContentResult>(response);

            await using var context = _fixture.CreateDbContext();
            var entity = await context.DonationRequests.AsNoTracking().SingleAsync(dr => dr.Id == seed.DonationRequestId);
            Assert.Equal(RequestStatus.Approved, entity.Status);
        }
        finally
        {
            await DeleteDonationSeedAsync(seed);
        }
    }

    [Fact]
    public async Task Reject_PersistsCancelledStatusAndReturnsNoContent()
    {
        var seed = await SeedDonationRequestAsync(RequestStatus.Pending);
        var controller = _fixture.CreateDonationRequestsController(Guid.NewGuid(), "Admin");

        try
        {
            var response = await controller.Reject(seed.DonationRequestId!.Value);

            Assert.IsType<NoContentResult>(response);

            await using var context = _fixture.CreateDbContext();
            var entity = await context.DonationRequests.AsNoTracking().SingleAsync(dr => dr.Id == seed.DonationRequestId);
            Assert.Equal(RequestStatus.Cancelled, entity.Status);
        }
        finally
        {
            await DeleteDonationSeedAsync(seed);
        }
    }

    [Fact]
    public async Task GetMyDonationRequests_WhenTokenHasNoUserId_ThrowsUnauthorizedAccessException()
    {
        var controller = _fixture.CreateDonationRequestsController();

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => controller.GetMyDonationRequests(new PageParameters()));

        Assert.Equal("User ID was not found in token.", exception.Message);
    }

    private async Task<DonationSeed> SeedDonationRequestAsync(RequestStatus status, Guid? donorId = null)
    {
        var seed = await SeedDonationDependenciesAsync(donorId);
        var donationRequestId = Guid.NewGuid();
        var title = $"Test donation request {donationRequestId:N}";

        await using var context = _fixture.CreateDbContext();
        context.DonationRequests.Add(new DonationRequest
        {
            Id = donationRequestId,
            Title = title,
            LocationId = seed.LocationId,
            DonateAmount = 100,
            AmountRemaining = 100,
            Status = status,
            UrgencyLevel = UrgencyLevel.Medium,
            BriefDescription = "Seeded by database-backed API test",
            PartnerId = seed.PartnerId,
            CreatedAt = DateTime.UtcNow
        });

        if (donorId.HasValue)
        {
            context.DonationOrders.Add(new DonationOrder
            {
                Id = Guid.NewGuid(),
                Amount = 25,
                Currency = "EGP",
                PaymentMethod = "Card",
                Category = "Education",
                DonorId = donorId.Value,
                DonationRequestId = donationRequestId,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
        return seed with { DonationRequestId = donationRequestId, Title = title };
    }

    private async Task<DonationSeed> SeedDonationDependenciesAsync(Guid? donorId = null)
    {
        var partnerId = Guid.NewGuid();
        var locationId = Guid.NewGuid();

        await using var context = _fixture.CreateDbContext();
        context.Partners.Add(new Partner
        {
            Id = partnerId,
            OrgName = $"Test partner {partnerId:N}",
            OrgType = OrgType.Foundation,
            CreatedAt = DateTime.UtcNow
        });
        context.Locations.Add(new Location
        {
            Id = locationId,
            Name = $"Test location {locationId:N}",
            RequiredLevel = 1,
            Latitude = "30.0444",
            Longitude = "31.2357",
            CreatedAt = DateTime.UtcNow
        });

        if (donorId.HasValue)
        {
            context.Users.Add(new ApplicationUser
            {
                Id = donorId.Value,
                UserName = $"donor-{donorId.Value:N}@tests.local",
                NormalizedUserName = $"DONOR-{donorId.Value:N}@TESTS.LOCAL",
                Email = $"donor-{donorId.Value:N}@tests.local",
                NormalizedEmail = $"DONOR-{donorId.Value:N}@TESTS.LOCAL",
                EmailConfirmed = true
            });
        }

        await context.SaveChangesAsync();
        return new DonationSeed(partnerId, locationId, null, donorId, null);
    }

    private async Task DeleteDonationSeedAsync(DonationSeed seed)
    {
        await using var context = _fixture.CreateDbContext();

        if (seed.DonationRequestId.HasValue)
        {
            await context.DonationOrders
                .Where(order => order.DonationRequestId == seed.DonationRequestId.Value)
                .ExecuteDeleteAsync();
            await context.DonationRequests
                .IgnoreQueryFilters()
                .Where(request => request.Id == seed.DonationRequestId.Value)
                .ExecuteDeleteAsync();
        }

        await context.Partners.Where(partner => partner.Id == seed.PartnerId).ExecuteDeleteAsync();
        await context.Locations.Where(location => location.Id == seed.LocationId).ExecuteDeleteAsync();

        if (seed.DonorId.HasValue)
            await context.Users.Where(user => user.Id == seed.DonorId.Value).ExecuteDeleteAsync();
    }

    private sealed record DonationSeed(
        Guid PartnerId,
        Guid LocationId,
        Guid? DonationRequestId,
        Guid? DonorId,
        string? Title);
}
