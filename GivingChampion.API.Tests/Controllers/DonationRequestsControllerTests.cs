using GivingChampion.API.Controllers;
using GivingChampion.API.Tests.Infrastructure;
using GivingChampion.Application.DTO.DonationRequest;
using GivingChampion.Application.Interfaces;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Tests.Controllers;

public class DonationRequestsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsPagedDonationRequestsFromService()
    {
        var dto = CreateDonationRequestDto();
        var page = CreatePage(dto);
        var service = new FakeDonationRequestService
        {
            GetAllResult = page
        };
        var controller = CreateController(service, Guid.NewGuid(), "Admin");

        var response = await controller.GetAll(new PageParameters { PageNumber = 1, PageSize = 100 });

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        var resultPage = Assert.IsType<PagedList<DonationRequestDto>>(okResult.Value);
        Assert.Same(page, resultPage);
    }

    [Fact]
    public async Task GetMyDonationRequests_UsesCurrentUserAndReturnsServiceResult()
    {
        var dto = CreateDonationRequestDto();
        var page = CreatePage(dto);
        var service = new FakeDonationRequestService
        {
            GetMyRequestsResult = page
        };
        var controller = CreateController(service, Guid.NewGuid(), "Donor");

        var response = await controller.GetMyDonationRequests(new PageParameters { PageNumber = 1, PageSize = 20 });

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        var resultPage = Assert.IsType<PagedList<DonationRequestDto>>(okResult.Value);
        Assert.Same(page, resultPage);
    }

    [Fact]
    public async Task GetById_WhenAdmin_ReturnsDonationRequest()
    {
        var donationRequestId = Guid.NewGuid();
        var dto = CreateDonationRequestDto(donationRequestId);
        var service = new FakeDonationRequestService
        {
            GetByIdResult = dto
        };
        var controller = CreateController(service, Guid.NewGuid(), "Admin");

        var response = await controller.GetById(donationRequestId);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        var resultDto = Assert.IsType<DonationRequestDto>(okResult.Value);
        Assert.Same(dto, resultDto);
        Assert.True(service.LastIsAdmin);
        Assert.Equal(donationRequestId, service.LastGetByIdId);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var createdDto = CreateDonationRequestDto(Guid.NewGuid(), RequestStatus.Pending.ToString());
        var service = new FakeDonationRequestService
        {
            AddResult = createdDto
        };
        var controller = CreateController(service, Guid.NewGuid(), "Admin");
        var request = new CreateDonationRequestDto
        {
            Title = "Create request",
            LocationId = Guid.NewGuid(),
            DonateAmount = 250,
            UrgencyLevel = UrgencyLevel.Medium,
            BriefDescription = "Unit test",
            PartnerId = Guid.NewGuid()
        };

        var response = await controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(response.Result);
        Assert.Equal(nameof(DonationRequestsController.GetById), createdResult.ActionName);
        Assert.Same(createdDto, createdResult.Value);
        Assert.Same(request, service.LastCreateDto);
        Assert.True(service.LastIsAdmin);
    }

    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        var donationRequestId = Guid.NewGuid();
        var service = new FakeDonationRequestService();
        var controller = CreateController(service, Guid.NewGuid(), "Admin");
        var update = new UpdateDonationRequestDto
        {
            Title = "Updated title",
            DonateAmount = 500
        };

        var response = await controller.Update(donationRequestId, update);

        Assert.IsType<NoContentResult>(response);
        Assert.Equal(donationRequestId, service.LastUpdatedId);
        Assert.Same(update, service.LastUpdateDto);
        Assert.True(service.LastIsAdmin);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var donationRequestId = Guid.NewGuid();
        var service = new FakeDonationRequestService();
        var controller = CreateController(service, Guid.NewGuid(), "Admin");

        var response = await controller.Delete(donationRequestId);

        Assert.IsType<NoContentResult>(response);
        Assert.Equal(donationRequestId, service.LastDeletedId);
        Assert.True(service.LastIsAdmin);
    }

    [Fact]
    public async Task Approve_ReturnsNoContent()
    {
        var donationRequestId = Guid.NewGuid();
        var service = new FakeDonationRequestService();
        var controller = CreateController(service, Guid.NewGuid(), "Admin");

        var response = await controller.Approve(donationRequestId);

        Assert.IsType<NoContentResult>(response);
        Assert.Equal(donationRequestId, service.LastApprovedId);
    }

    [Fact]
    public async Task Reject_ReturnsNoContent()
    {
        var donationRequestId = Guid.NewGuid();
        var service = new FakeDonationRequestService();
        var controller = CreateController(service, Guid.NewGuid(), "Admin");

        var response = await controller.Reject(donationRequestId);

        Assert.IsType<NoContentResult>(response);
        Assert.Equal(donationRequestId, service.LastRejectedId);
    }

    [Fact]
    public async Task GetMyDonationRequests_WhenServiceThrowsUnauthorized_PropagatesException()
    {
        var service = new FakeDonationRequestService
        {
            GetMyRequestsException = new UnauthorizedAccessException("User ID was not found in token.")
        };
        var controller = CreateController(service);

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => controller.GetMyDonationRequests(new PageParameters()));

        Assert.Equal("User ID was not found in token.", exception.Message);
    }

    private static DonationRequestsController CreateController(
        IDonationRequestService service,
        Guid? userId = null,
        params string[] roles)
    {
        var controller = new DonationRequestsController(service)
        {
            ControllerContext = TestAuthContextFactory.CreateControllerContext(userId, roles)
        };

        return controller;
    }

    private static DonationRequestDto CreateDonationRequestDto(
        Guid? id = null,
        string status = "Pending")
    {
        return new DonationRequestDto
        {
            Id = id ?? Guid.NewGuid(),
            Title = "Donation request",
            Status = status,
            DonateAmount = 100,
            AmountRemaining = 100,
            BriefDescription = "Test"
        };
    }

    private static PagedList<DonationRequestDto> CreatePage(params DonationRequestDto[] items)
        => TestPaging.CreatePage(items);

    private sealed class FakeDonationRequestService : IDonationRequestService
    {
        public PagedList<DonationRequestDto>? GetAllResult { get; set; }
        public PagedList<DonationRequestDto>? GetApprovedResult { get; set; }
        public PagedList<DonationRequestDto>? GetMyRequestsResult { get; set; }
        public DonationRequestDto? GetByIdResult { get; set; }
        public DonationRequestDto? AddResult { get; set; }
        public Exception? GetMyRequestsException { get; set; }
        public Guid LastGetByIdId { get; private set; }
        public bool LastIsAdmin { get; private set; }
        public CreateDonationRequestDto? LastCreateDto { get; private set; }
        public Guid LastUpdatedId { get; private set; }
        public UpdateDonationRequestDto? LastUpdateDto { get; private set; }
        public Guid LastApprovedId { get; private set; }
        public Guid LastRejectedId { get; private set; }
        public Guid LastDeletedId { get; private set; }

        public Task<PagedList<DonationRequestDto>> GetAllAsync(PageParameters paginationParams)
            => Task.FromResult(GetAllResult ?? CreatePage());

        public Task<PagedList<DonationRequestDto>> GetApprovedAsync(PageParameters paginationParams)
            => Task.FromResult(GetApprovedResult ?? CreatePage());

        public Task<PagedList<DonationRequestDto>> GetMyRequestsAsync(PageParameters paginationParams)
        {
            if (GetMyRequestsException != null)
                throw GetMyRequestsException;

            return Task.FromResult(GetMyRequestsResult ?? CreatePage());
        }

        public Task<DonationRequestDto> GetByIdAsync(Guid id, bool isAdmin)
        {
            LastGetByIdId = id;
            LastIsAdmin = isAdmin;
            return Task.FromResult(GetByIdResult!);
        }

        public Task<DonationRequestDto> AddAsync(CreateDonationRequestDto dto, bool isAdmin = false)
        {
            LastCreateDto = dto;
            LastIsAdmin = isAdmin;
            return Task.FromResult(AddResult!);
        }

        public Task UpdateAsync(Guid id, UpdateDonationRequestDto dto, bool isAdmin = false)
        {
            LastUpdatedId = id;
            LastUpdateDto = dto;
            LastIsAdmin = isAdmin;
            return Task.CompletedTask;
        }

        public Task ApproveAsync(Guid id)
        {
            LastApprovedId = id;
            return Task.CompletedTask;
        }

        public Task RejectAsync(Guid id)
        {
            LastRejectedId = id;
            return Task.CompletedTask;
        }

        public Task SoftDeleteAsync(Guid id, bool isAdmin)
        {
            LastDeletedId = id;
            LastIsAdmin = isAdmin;
            return Task.CompletedTask;
        }
    }
}
