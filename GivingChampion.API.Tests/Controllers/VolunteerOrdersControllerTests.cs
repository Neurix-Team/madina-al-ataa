using GivingChampion.API.Controllers.VolunteerOrder;
using GivingChampion.Application.DTO.VolunteerOrder;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Tests.Controllers;

public class VolunteerOrdersControllerTests
{
    [Fact]
    public async Task GetAll_WhenServiceSucceeds_ReturnsOk()
    {
        var dto = CreateVolunteerOrderDto();
        var page = CreatePage(dto);
        var service = new FakeVolunteerOrderService
        {
            GetAllResult = Result<PagedList<VolunteerOrderDto>>.Success(page)
        };
        var controller = new VolunteerOrdersController(service);

        var result = await controller.GetAll(new PageParameters { PageNumber = 1, PageSize = 10 });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(service.GetAllResult, okResult.Value);
    }

    [Fact]
    public async Task GetAll_WhenServiceFails_ReturnsBadRequest()
    {
        var service = new FakeVolunteerOrderService
        {
            GetAllResult = Result<PagedList<VolunteerOrderDto>>.Failure("failed")
        };
        var controller = new VolunteerOrdersController(service);

        var result = await controller.GetAll(new PageParameters { PageNumber = 1, PageSize = 10 });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Same(service.GetAllResult, badRequest.Value);
    }

    [Fact]
    public async Task GetPending_WhenServiceSucceeds_ReturnsOk()
    {
        var dto = CreateVolunteerOrderDto();
        var page = CreatePage(dto);
        var service = new FakeVolunteerOrderService
        {
            GetPendingResult = Result<PagedList<VolunteerOrderDto>>.Success(page)
        };
        var controller = new VolunteerOrdersController(service);

        var result = await controller.GetPending(new PageParameters { PageNumber = 1, PageSize = 10 });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(service.GetPendingResult, okResult.Value);
    }

    [Fact]
    public async Task GetPendingCount_WhenServiceSucceeds_ReturnsOk()
    {
        var count = new PendingVolunteerOrderCountDto { Exists = true, Count = 3 };
        var service = new FakeVolunteerOrderService
        {
            GetPendingCountResult = Result<PendingVolunteerOrderCountDto>.Success(count)
        };
        var controller = new VolunteerOrdersController(service);

        var result = await controller.GetPendingCount();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(service.GetPendingCountResult, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsServiceResult()
    {
        var volunteerOrderId = Guid.NewGuid();
        var dto = CreateVolunteerOrderDto(volunteerOrderId);
        var service = new FakeVolunteerOrderService
        {
            GetByIdResult = dto
        };
        var controller = new VolunteerOrdersController(service);

        var response = await controller.GetById(volunteerOrderId);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(dto, okResult.Value);
        Assert.Equal(volunteerOrderId, service.LastGetByIdId);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var createdDto = CreateVolunteerOrderDto();
        var request = new CreateVolunteerOrderDto { ServiceRequestId = Guid.NewGuid() };
        var service = new FakeVolunteerOrderService
        {
            CreateResult = createdDto
        };
        var controller = new VolunteerOrdersController(service);

        var response = await controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(response.Result);
        Assert.Equal(nameof(VolunteerOrdersController.GetById), createdResult.ActionName);
        Assert.Equal(createdDto.Id, createdResult.RouteValues?["id"]);
        Assert.Same(createdDto, createdResult.Value);
        Assert.Same(request, service.LastCreateDto);
    }

    [Fact]
    public async Task ApproveOrder_ReturnsOkWithServiceResult()
    {
        var volunteerOrderId = Guid.NewGuid();
        var dto = CreateVolunteerOrderDto(volunteerOrderId, "Approved");
        var service = new FakeVolunteerOrderService
        {
            ApproveResult = dto
        };
        var controller = new VolunteerOrdersController(service);

        var result = await controller.ApproveOrder(volunteerOrderId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(dto, okResult.Value);
        Assert.Equal(volunteerOrderId, service.LastApprovedId);
    }

    [Fact]
    public async Task RejectOrder_UsesRejectionReasonAndReturnsOk()
    {
        var volunteerOrderId = Guid.NewGuid();
        var dto = CreateVolunteerOrderDto(volunteerOrderId, "Rejected");
        var request = new RejectVolunteerOrderDto { RejectionReason = "No capacity" };
        var service = new FakeVolunteerOrderService
        {
            RejectResult = dto
        };
        var controller = new VolunteerOrdersController(service);

        var result = await controller.RejectOrder(volunteerOrderId, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(dto, okResult.Value);
        Assert.Equal(volunteerOrderId, service.LastRejectedId);
        Assert.Equal(request.RejectionReason, service.LastRejectionReason);
    }

    [Fact]
    public async Task UpdateProgress_UsesProgressAndReturnsOk()
    {
        var volunteerOrderId = Guid.NewGuid();
        var dto = CreateVolunteerOrderDto(volunteerOrderId, "InProgress");
        var request = new UpdateVolunteerOrderProgressDto { Progress = 60 };
        var service = new FakeVolunteerOrderService
        {
            UpdateProgressResult = dto
        };
        var controller = new VolunteerOrdersController(service);

        var result = await controller.UpdateProgress(volunteerOrderId, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(dto, okResult.Value);
        Assert.Equal(volunteerOrderId, service.LastUpdatedProgressId);
        Assert.Equal(request.Progress, service.LastProgress);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var volunteerOrderId = Guid.NewGuid();
        var service = new FakeVolunteerOrderService();
        var controller = new VolunteerOrdersController(service);

        var result = await controller.Delete(volunteerOrderId);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(volunteerOrderId, service.LastDeletedId);
    }

    private static VolunteerOrderDto CreateVolunteerOrderDto(
        Guid? id = null,
        string status = "Pending")
    {
        return new VolunteerOrderDto
        {
            Id = id ?? Guid.NewGuid(),
            ServiceType = "Teaching",
            Progress = 0,
            Description = "Test order",
            ScheduleDate = DateTime.UtcNow.AddDays(1),
            Status = status,
            ServiceRequestId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private static PagedList<VolunteerOrderDto> CreatePage(params VolunteerOrderDto[] items)
        => new(items, 1, items.Length == 0 ? 10 : items.Length, items.Length);

    private sealed class FakeVolunteerOrderService : IVolunteerOrderService
    {
        public Result<PagedList<VolunteerOrderDto>>? GetAllResult { get; set; }
        public Result<PagedList<VolunteerOrderDto>>? GetPendingResult { get; set; }
        public Result<PendingVolunteerOrderCountDto>? GetPendingCountResult { get; set; }
        public VolunteerOrderDto? GetByIdResult { get; set; }
        public VolunteerOrderDto? CreateResult { get; set; }
        public VolunteerOrderDto? UpdateProgressResult { get; set; }
        public VolunteerOrderDto? ApproveResult { get; set; }
        public VolunteerOrderDto? RejectResult { get; set; }
        public Guid LastGetByIdId { get; private set; }
        public CreateVolunteerOrderDto? LastCreateDto { get; private set; }
        public Guid LastUpdatedProgressId { get; private set; }
        public int LastProgress { get; private set; }
        public Guid LastApprovedId { get; private set; }
        public Guid LastRejectedId { get; private set; }
        public string? LastRejectionReason { get; private set; }
        public Guid LastDeletedId { get; private set; }

        public Task<Result<PagedList<VolunteerOrderDto>>> GetAllAsync(PageParameters pageParameters)
            => Task.FromResult(GetAllResult ?? Result<PagedList<VolunteerOrderDto>>.Success(CreatePage()));

        public Task<Result<PagedList<VolunteerOrderDto>>> GetPendingAsync(PageParameters pageParameters)
            => Task.FromResult(GetPendingResult ?? Result<PagedList<VolunteerOrderDto>>.Success(CreatePage()));

        public Task<Result<PagedList<VolunteerOrderDto>>> GetMyOrdersAsync(PageParameters pageParameters)
            => Task.FromResult(Result<PagedList<VolunteerOrderDto>>.Success(CreatePage()));

        public Task<Result<PendingVolunteerOrderCountDto>> GetPendingCountAsync()
            => Task.FromResult(GetPendingCountResult ?? Result<PendingVolunteerOrderCountDto>.Success(new PendingVolunteerOrderCountDto()));

        public Task<VolunteerOrderDto?> GetByIdAsync(Guid id)
        {
            LastGetByIdId = id;
            return Task.FromResult(GetByIdResult);
        }

        public Task<VolunteerOrderDto> CreateAsync(CreateVolunteerOrderDto dto)
        {
            LastCreateDto = dto;
            return Task.FromResult(CreateResult!);
        }

        public Task<VolunteerOrderDto?> UpdateProgressAsync(Guid orderId, int progress)
        {
            LastUpdatedProgressId = orderId;
            LastProgress = progress;
            return Task.FromResult(UpdateProgressResult);
        }

        public Task<VolunteerOrderDto?> ApproveOrderAsync(Guid id)
        {
            LastApprovedId = id;
            return Task.FromResult(ApproveResult);
        }

        public Task<VolunteerOrderDto?> RejectOrderAsync(Guid id, string rejectionReason)
        {
            LastRejectedId = id;
            LastRejectionReason = rejectionReason;
            return Task.FromResult(RejectResult);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            LastDeletedId = id;
            return Task.FromResult(true);
        }
    }
}
