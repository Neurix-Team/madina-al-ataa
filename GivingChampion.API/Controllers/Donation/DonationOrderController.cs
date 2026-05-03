using GivingChampion.Application.Interfaces.DonationOrderService;
using GivingChampion.Application.DTO.DonationOrder;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Route("api/donation-orders")]
    [ApiController]
    [Authorize]
    public class DonationOrdersController : ControllerBase
    {
        private readonly IDonationOrderService _donationOrderService;

        public DonationOrdersController(IDonationOrderService donationOrderService)
        {
            _donationOrderService = donationOrderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<PagedList<DonationOrderReadDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<Result<PagedList<DonationOrderReadDto>>> GetAllDonationOrders(
            [FromQuery] PageParameters pageParameters)
        {
            return await _donationOrderService.GetAllAsync(pageParameters);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Donor")]
        [ProducesResponseType(typeof(Result<PagedList<DonationOrderReadDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<Result<PagedList<DonationOrderReadDto>>> GetMyDonationOrders(
            [FromQuery] PageParameters pageParameters)
        {
            return await _donationOrderService.GetMyOrdersAsync(pageParameters);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Donor,Admin")]
        [ProducesResponseType(typeof(Result<DonationOrderDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result<DonationOrderDetailsDto>> GetDonationOrderById(Guid id)
        {
            return await _donationOrderService.GetByIdAsync(id, User.IsInRole("Admin"));
        }

        [HttpPost]
        [Authorize(Roles = "Donor")]
        [ProducesResponseType(typeof(Result<DonationOrderDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result<DonationOrderDetailsDto>> CreateDonationOrder(
            [FromBody] CreateDonationOrderDto donationOrderDto)
        {
            return await _donationOrderService.CreateAsync(donationOrderDto);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Donor")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result> UpdateDonationOrder(
            Guid id,
            [FromBody] UpdateDonationOrderDTO donationOrderDto)
        {
            return await _donationOrderService.UpdateAsync(id, donationOrderDto);
        }

        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<DonationOrderDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result> ApproveDonationOrder(Guid id)
        {
            return await _donationOrderService.ApproveAsync(id);
        }

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<DonationOrderDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result> RejectDonationOrder(Guid id)
        {
            return await _donationOrderService.RejectAsync(id);
        }
    }
}
