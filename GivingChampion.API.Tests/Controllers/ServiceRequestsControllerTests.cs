using GivingChampion.API.Controllers;
using GivingChampion.Application.DTO.ServiceRequestDto;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Tests.Controllers;

public class ServiceRequestsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkWithServiceResult()
    {
        var dto = CreateServiceRequestDto();
        var page = CreatePage(dto);
        var service = new FakeServiceRequestService
        {
            GetAllResult = Result<PagedList<ServiceRequestDto>>.Success(page)
        };
        var controller = new ServiceRequestsController(service);

        var result = await controller.GetAll(new PageParameters { PageNumber = 1, PageSize = 10 });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(service.GetAllResult, okResult.Value);
    }

    [Fact]
    public async Task GetApproved_ReturnsOkWithServiceResult()
    {
        var dto = CreateServiceRequestDto(status: "Approved");
        var page = CreatePage(dto);
        var service = new FakeServiceRequestService
        {
            GetApprovedResult = Result<PagedList<ServiceRequestDto>>.Success(page)
        };
        var controller = new ServiceRequestsController(service);

        var result = await controller.GetApproved(new PageParameters { PageNumber = 1, PageSize = 10 });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(service.GetApprovedResult, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOkWithServiceRequest()
    {
        var serviceRequestId = Guid.NewGuid();
        var dto = CreateServiceRequestDto(serviceRequestId);
        var service = new FakeServiceRequestService
        {
            GetByIdResult = dto
        };
        var controller = new ServiceRequestsController(service);

        var result = await controller.GetById(serviceRequestId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(dto, okResult.Value);
        Assert.Equal(serviceRequestId, service.LastGetByIdId);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var createdDto = CreateServiceRequestDto();
        var request = CreateCreateServiceRequestDto();
        var service = new FakeServiceRequestService
        {
            CreateResult = createdDto
        };
        var controller = new ServiceRequestsController(service);

        var result = await controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ServiceRequestsController.GetById), createdResult.ActionName);
        Assert.Equal(createdDto.Id, createdResult.RouteValues?["id"]);
        Assert.Same(createdDto, createdResult.Value);
        Assert.Same(request, service.LastCreateDto);
    }

    [Fact]
    public async Task Update_WhenServiceSucceeds_ReturnsNoContent()
    {
        var serviceRequestId = Guid.NewGuid();
        var request = CreateUpdateServiceRequestDto();
        var service = new FakeServiceRequestService
        {
            UpdateResult = true
        };
        var controller = new ServiceRequestsController(service);

        var result = await controller.Update(serviceRequestId, request);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(serviceRequestId, service.LastUpdatedId);
        Assert.Same(request, service.LastUpdateDto);
    }

    [Fact]
    public async Task Update_WhenServiceReturnsFalse_ThrowsNotFoundException()
    {
        var service = new FakeServiceRequestService
        {
            UpdateResult = false
        };
        var controller = new ServiceRequestsController(service);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => controller.Update(Guid.NewGuid(), CreateUpdateServiceRequestDto()));

        Assert.Equal("Service request not found.", exception.Message);
    }

    [Fact]
    public async Task Delete_WhenServiceSucceeds_ReturnsNoContent()
    {
        var serviceRequestId = Guid.NewGuid();
        var service = new FakeServiceRequestService
        {
            DeleteResult = true
        };
        var controller = new ServiceRequestsController(service);

        var result = await controller.Delete(serviceRequestId);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(serviceRequestId, service.LastDeletedId);
    }

    [Fact]
    public async Task Delete_WhenServiceReturnsFalse_ThrowsNotFoundException()
    {
        var service = new FakeServiceRequestService
        {
            DeleteResult = false
        };
        var controller = new ServiceRequestsController(service);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => controller.Delete(Guid.NewGuid()));

        Assert.Equal("Service request not found.", exception.Message);
    }

    private static ServiceRequestDto CreateServiceRequestDto(
        Guid? id = null,
        string status = "Pending")
    {
        return new ServiceRequestDto
        {
            Id = id ?? Guid.NewGuid(),
            Title = "Service request",
            RequiredSkill = "Teaching",
            UrgencyLevel = UrgencyLevel.Medium.ToString(),
            ScheduleDate = DateTime.UtcNow.AddDays(1),
            Duration = 2,
            BriefDescription = "Test request",
            ServiceType = "Education",
            PartnerId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            FullName = "Partner",
            Status = status,
            CreatedAt = DateTime.UtcNow,
            MaxOrders = 5,
            RequiredLevelId = Guid.NewGuid()
        };
    }

    private static CreateServiceRequestDto CreateCreateServiceRequestDto()
        => new()
        {
            Title = "Service request",
            ServiceType = "Education",
            RequiredSkill = "Teaching",
            UrgencyLevel = UrgencyLevel.Medium,
            ScheduleDate = DateTime.UtcNow.AddDays(1),
            Duration = 2,
            BriefDescription = "Test request",
            PartnerId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            MaxOrders = 5,
            RequiredLevelId = Guid.NewGuid()
        };

    private static UpdateServiceRequestDto CreateUpdateServiceRequestDto()
        => new()
        {
            Title = "Updated request",
            RequiredSkill = "Teaching",
            UrgencyLevel = UrgencyLevel.High,
            ScheduleDate = DateTime.UtcNow.AddDays(2),
            Duration = 3,
            BriefDescription = "Updated request",
            LocationId = Guid.NewGuid(),
            MaxOrders = 6,
            RequiredLevelId = Guid.NewGuid()
        };

    private static PagedList<ServiceRequestDto> CreatePage(params ServiceRequestDto[] items)
        => new(items, 1, items.Length == 0 ? 10 : items.Length, items.Length);

    private sealed class FakeServiceRequestService : IServiceRequestService
    {
        public Result<PagedList<ServiceRequestDto>>? GetAllResult { get; set; }
        public Result<PagedList<ServiceRequestDto>>? GetApprovedResult { get; set; }
        public ServiceRequestDto? GetByIdResult { get; set; }
        public ServiceRequestDto? CreateResult { get; set; }
        public bool UpdateResult { get; set; } = true;
        public bool DeleteResult { get; set; } = true;
        public Guid LastGetByIdId { get; private set; }
        public CreateServiceRequestDto? LastCreateDto { get; private set; }
        public Guid LastUpdatedId { get; private set; }
        public UpdateServiceRequestDto? LastUpdateDto { get; private set; }
        public Guid LastDeletedId { get; private set; }

        public Task<Result<PagedList<ServiceRequestDto>>> GetAllAsync(PageParameters pageParameters)
            => Task.FromResult(GetAllResult ?? Result<PagedList<ServiceRequestDto>>.Success(CreatePage()));

        public Task<ServiceRequestDto?> GetByIdAsync(Guid id)
        {
            LastGetByIdId = id;
            return Task.FromResult(GetByIdResult);
        }

        public Task<Result<PagedList<ServiceRequestDto>>> GetApprovedRequestsAsync(PageParameters pageParameters)
            => Task.FromResult(GetApprovedResult ?? Result<PagedList<ServiceRequestDto>>.Success(CreatePage()));

        public Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto)
        {
            LastCreateDto = dto;
            return Task.FromResult(CreateResult!);
        }

        public Task<bool> UpdateAsync(Guid id, UpdateServiceRequestDto dto)
        {
            LastUpdatedId = id;
            LastUpdateDto = dto;
            return Task.FromResult(UpdateResult);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            LastDeletedId = id;
            return Task.FromResult(DeleteResult);
        }
    }
}
