using GivingChampion.API.Tests.Infrastructure;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Tests.Controllers;

[Collection(DatabaseCollection.Name)]
public class VolunteerControllerTests
{
    private readonly DatabaseTestFixture _fixture;

    public VolunteerControllerTests(DatabaseTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetById_WhenVolunteerExists_ReturnsOkWithVolunteerFromDatabase()
    {
        var userId = Guid.NewGuid();
        var volunteerId = Guid.NewGuid();
        await SeedVolunteerAsync(userId, volunteerId);
        var controller = _fixture.CreateVolunteerController();

        try
        {
            var result = await controller.GetById(volunteerId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var volunteer = Assert.IsType<Application.DTO.VolunteerDto.VolunteerDto>(okResult.Value);
            Assert.Equal(volunteerId, volunteer.Id);
            Assert.Equal(userId, volunteer.UserId);
            Assert.Equal("Teaching", volunteer.Skills);
            Assert.Equal("Weekends", volunteer.Availability);
            Assert.Equal(12, volunteer.TotalHours);
        }
        finally
        {
            await DeleteVolunteerSeedAsync(userId, volunteerId);
        }
    }

    [Fact]
    public async Task GetById_WhenVolunteerDoesNotExist_ReturnsInternalServerErrorFromServiceException()
    {
        var controller = _fixture.CreateVolunteerController();

        var result = await controller.GetById(Guid.NewGuid());

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Internal server error", objectResult.Value);
    }

    private async Task SeedVolunteerAsync(Guid userId, Guid volunteerId)
    {
        await using var context = _fixture.CreateDbContext();
        context.Users.Add(new ApplicationUser
        {
            Id = userId,
            UserName = $"volunteer-{userId:N}@tests.local",
            NormalizedUserName = $"VOLUNTEER-{userId:N}@TESTS.LOCAL",
            Email = $"volunteer-{userId:N}@tests.local",
            NormalizedEmail = $"VOLUNTEER-{userId:N}@TESTS.LOCAL",
            EmailConfirmed = true
        });
        context.Volunteers.Add(new Volunteer
        {
            Id = volunteerId,
            UserId = userId,
            Skills = "Teaching",
            Availability = "Weekends",
            TotalHours = 12,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }

    private async Task DeleteVolunteerSeedAsync(Guid userId, Guid volunteerId)
    {
        await using var context = _fixture.CreateDbContext();
        await context.Volunteers.Where(v => v.Id == volunteerId).ExecuteDeleteAsync();
        await context.Users.Where(u => u.Id == userId).ExecuteDeleteAsync();
    }
}
