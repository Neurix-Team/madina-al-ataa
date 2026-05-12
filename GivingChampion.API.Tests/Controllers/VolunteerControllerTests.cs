using GivingChampion.API.Controllers;
using GivingChampion.Application.DTO.Volunteer;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Tests.Controllers;

public class VolunteerControllerTests
{
    [Fact]
    public async Task GetMyVolunteerProfile_ReturnsOkWithServiceResult()
    {
        var dto = new VolunteerDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Skills = "Teaching",
            Availability = "Weekends",
            TotalHours = 12
        };
        var expected = Result<VolunteerDto>.Success(dto);
        var service = new FakeVolunteerService
        {
            GetMyVolunteerProfileResult = expected
        };
        var controller = new VolunteerController(service);

        var response = await controller.GetMyVolunteerProfile();

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(expected, okResult.Value);
    }

    [Fact]
    public async Task GetVolunteerByUserId_ReturnsOkWithServiceResult()
    {
        var userId = Guid.NewGuid();
        var dto = new VolunteerDto
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Skills = "Teaching",
            Availability = "Weekends",
            TotalHours = 12
        };
        var expected = Result<VolunteerDto>.Success(dto);
        var service = new FakeVolunteerService
        {
            GetVolunteerByUserIdResult = expected
        };
        var controller = new VolunteerController(service);

        var response = await controller.GetVolunteerByUserId(userId);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(expected, okResult.Value);
        Assert.Equal(userId, service.LastUserId);
    }

    [Fact]
    public async Task UpdateMyVolunteerProfile_ReturnsOkWithServiceResult()
    {
        var request = new UpdateVolunteerDto
        {
            Skills = "Teaching",
            Availability = "Weekdays"
        };
        var dto = new VolunteerDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Skills = request.Skills,
            Availability = request.Availability,
            TotalHours = 12
        };
        var expected = Result<VolunteerDto>.Success(dto);
        var service = new FakeVolunteerService
        {
            UpdateVolunteerResult = expected
        };
        var controller = new VolunteerController(service);

        var response = await controller.UpdateMyVolunteerProfile(request);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(expected, okResult.Value);
        Assert.Same(request, service.LastUpdateDto);
    }

    private sealed class FakeVolunteerService : IVolunteerService
    {
        public Result<VolunteerDto>? GetMyVolunteerProfileResult { get; set; }
        public Result<VolunteerDto>? GetVolunteerByUserIdResult { get; set; }
        public Result<VolunteerDto>? UpdateVolunteerResult { get; set; }
        public Guid LastUserId { get; private set; }
        public UpdateVolunteerDto? LastUpdateDto { get; private set; }

        public Task<Result<VolunteerDto>> GetMyVolunteerProfileAsync()
            => Task.FromResult(GetMyVolunteerProfileResult!);

        public Task<Result<VolunteerDto>> GetVolunteerByUserIdAsync(Guid userId)
        {
            LastUserId = userId;
            return Task.FromResult(GetVolunteerByUserIdResult!);
        }

        public Task<Result<VolunteerDto>> UpdateVolunteerAsync(UpdateVolunteerDto dto)
        {
            LastUpdateDto = dto;
            return Task.FromResult(UpdateVolunteerResult!);
        }
    }
}
