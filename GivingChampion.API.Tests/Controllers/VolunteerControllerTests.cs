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
        var expected = Result<VolunteerDto>.Success(new VolunteerDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Skills = "Teaching",
            Availability = "Weekends",
            TotalHours = 12
        });
        var service = new FakeVolunteerService
        {
            MyVolunteerProfileResult = expected
        };
        var controller = new VolunteerController(service);

        var result = await controller.GetMyVolunteerProfile();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, okResult.Value);
    }

    [Fact]
    public async Task GetVolunteerByUserId_ReturnsOkWithServiceResult()
    {
        var userId = Guid.NewGuid();
        var expected = Result<VolunteerDto>.Success(new VolunteerDto
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Skills = "Teaching",
            Availability = "Weekends",
            TotalHours = 12
        });
        var service = new FakeVolunteerService
        {
            VolunteerByUserIdResult = expected
        };
        var controller = new VolunteerController(service);

        var result = await controller.GetVolunteerByUserId(userId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, okResult.Value);
        Assert.Equal(userId, service.LastRequestedUserId);
    }

    [Fact]
    public async Task UpdateMyVolunteerProfile_ReturnsOkWithServiceResult()
    {
        var updateDto = new UpdateVolunteerDto
        {
            Skills = "Mentoring",
            Availability = "Weekdays"
        };
        var expected = Result<VolunteerDto>.Success(new VolunteerDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Skills = updateDto.Skills,
            Availability = updateDto.Availability,
            TotalHours = 20
        });
        var service = new FakeVolunteerService
        {
            UpdateVolunteerResult = expected
        };
        var controller = new VolunteerController(service);

        var result = await controller.UpdateMyVolunteerProfile(updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, okResult.Value);
        Assert.Same(updateDto, service.LastUpdateDto);
    }

    private sealed class FakeVolunteerService : IVolunteerService
    {
        public Result<VolunteerDto>? MyVolunteerProfileResult { get; set; }
        public Result<VolunteerDto>? VolunteerByUserIdResult { get; set; }
        public Result<VolunteerDto>? UpdateVolunteerResult { get; set; }
        public Guid LastRequestedUserId { get; private set; }
        public UpdateVolunteerDto? LastUpdateDto { get; private set; }

        public Task<Result<VolunteerDto>> GetMyVolunteerProfileAsync()
            => Task.FromResult(MyVolunteerProfileResult!);

        public Task<Result<VolunteerDto>> GetVolunteerByUserIdAsync(Guid userId)
        {
            LastRequestedUserId = userId;
            return Task.FromResult(VolunteerByUserIdResult!);
        }

        public Task<Result<VolunteerDto>> UpdateVolunteerAsync(UpdateVolunteerDto dto)
        {
            LastUpdateDto = dto;
            return Task.FromResult(UpdateVolunteerResult!);
        }
    }
}
