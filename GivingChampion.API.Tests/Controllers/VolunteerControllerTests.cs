using GivingChampion.API.Controllers;
using GivingChampion.Application.DTO.Volunteer;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Tests.Controllers;

public class VolunteerControllerTests
{
    [Fact]
    public async Task GetById_WhenVolunteerExists_ReturnsOkWithVolunteer()
    {
        var volunteerId = Guid.NewGuid();
        var dto = new VolunteerDto
        {
            Id = volunteerId,
            UserId = Guid.NewGuid(),
            Skills = "Teaching",
            Availability = "Weekends",
            TotalHours = 12
        };
        var service = new FakeVolunteerService
        {
            Volunteer = dto
        };
        var controller = new VolunteerController(service);

        var result = await controller.GetById(volunteerId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var volunteer = Assert.IsType<VolunteerDto>(okResult.Value);
        Assert.Same(dto, volunteer);
        Assert.Equal(volunteerId, service.LastRequestedId);
    }

    [Fact]
    public async Task GetById_WhenVolunteerDoesNotExist_ThrowsNotFoundException()
    {
        var service = new FakeVolunteerService();
        var controller = new VolunteerController(service);
        var volunteerId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => controller.GetById(volunteerId));

        Assert.Equal($"Volunteer with ID {volunteerId} not found.", exception.Message);
    }

    [Fact]
    public async Task GetAll_WhenServiceSucceeds_ReturnsOk()
    {
        var dto = new VolunteerDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Skills = "Teaching",
            Availability = "Weekends",
            TotalHours = 12
        };
        var page = new PagedList<VolunteerDto>([dto], 1, 10, 1);
        var service = new FakeVolunteerService
        {
            GetAllResult = Result<PagedList<VolunteerDto>>.Success(page)
        };
        var controller = new VolunteerController(service);

        var result = await controller.GetAll(new PageParameters { PageNumber = 1, PageSize = 10 });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(service.GetAllResult, okResult.Value);
    }

    [Fact]
    public async Task GetAll_WhenServiceFails_ReturnsBadRequest()
    {
        var service = new FakeVolunteerService
        {
            GetAllResult = Result<PagedList<VolunteerDto>>.Failure("failed")
        };
        var controller = new VolunteerController(service);

        var result = await controller.GetAll(new PageParameters { PageNumber = 1, PageSize = 10 });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Same(service.GetAllResult, badRequest.Value);
    }

    private sealed class FakeVolunteerService : IVolunteerService
    {
        public VolunteerDto? Volunteer { get; set; }
        public Result<PagedList<VolunteerDto>>? GetAllResult { get; set; }
        public Guid LastRequestedId { get; private set; }

        public Task<Result<PagedList<VolunteerDto>>> GetAllAsync(PageParameters pageParameters)
            => Task.FromResult(GetAllResult!);

        public Task<VolunteerDto?> GetByIdAsync(Guid id)
        {
            LastRequestedId = id;
            return Task.FromResult(Volunteer);
        }
    }
}
