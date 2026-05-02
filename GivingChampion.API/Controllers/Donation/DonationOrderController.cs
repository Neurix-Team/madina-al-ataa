using GivingChampion.Application.Interfaces.DonationOrderService;
using GivingChampion.Application.DTO.DonationOrder;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GivingChampion.API.Controllers
{
    /// <summary>
    /// Manages donation order operations such as creating donation orders,
    /// viewing donation history, updating pending orders, and approving or rejecting orders.
    /// </summary>
    [Route("api/donation-orders")]
    [ApiController]
    [Authorize]
    public class DonationOrdersController : ControllerBase
    {
        private readonly IDonationOrderService _donationOrderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonationOrdersController"/> class.
        /// </summary>
        /// <param name="donationOrderService">The donation order service.</param>
        public DonationOrdersController(IDonationOrderService donationOrderService)
        {
            _donationOrderService = donationOrderService;
        }

        #region Query Methods

        /// <summary>
        /// Gets all donation orders in the system.
        /// </summary>
        /// <remarks>
        /// This endpoint is available only for admins.
        /// It returns paginated donation orders.
        /// </remarks>
        /// <param name="pageParameters">Pagination parameters such as page number and page size.</param>
        /// <returns>A paginated list of donation orders.</returns>
        /// <response code="200">Returns the paginated donation orders result.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have Admin role.</response>
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

        /// <summary>
        /// Gets the current donor's donation orders.
        /// </summary>
        /// <remarks>
        /// This endpoint returns only the donation orders that belong to the authenticated donor.
        /// </remarks>
        /// <param name="pageParameters">Pagination parameters such as page number and page size.</param>
        /// <returns>A paginated list of the current donor's donation orders.</returns>
        /// <response code="200">Returns the donor donation orders result.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have Donor role.</response>
        [HttpGet("my")]
        [Authorize(Roles = "Donor")]
        [ProducesResponseType(typeof(Result<PagedList<DonationOrderReadDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<Result<PagedList<DonationOrderReadDto>>> GetMyDonationOrders(
            [FromQuery] PageParameters pageParameters)
        {
            var userId = GetCurrentUserId();

            return await _donationOrderService.GetMyOrdersAsync(userId, pageParameters);
        }

        /// <summary>
        /// Gets a donation order by its unique identifier.
        /// </summary>
        /// <remarks>
        /// Donors can view only their own donation orders.
        /// Admins can view any donation order.
        /// </remarks>
        /// <param name="id">The donation order unique identifier.</param>
        /// <returns>The donation order details.</returns>
        /// <response code="200">Returns the donation order details result.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not allowed to view this donation order.</response>
        /// <response code="404">Donation order was not found.</response>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Donor,Admin")]
        [ProducesResponseType(typeof(Result<DonationOrderDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result<DonationOrderDetailsDto>> GetDonationOrderById(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            return await _donationOrderService.GetByIdAsync(id, userId, isAdmin);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Creates a new donation order.
        /// </summary>
        /// <remarks>
        /// This endpoint is available only for donors.
        /// The donation order is created for the authenticated donor.
        /// </remarks>
        /// <param name="donationOrderDto">The donation order creation data.</param>
        /// <returns>The created donation order details.</returns>
        /// <response code="200">Returns the created donation order result.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have Donor role.</response>
        /// <response code="404">Related donation request was not found.</response>
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
            var userId = GetCurrentUserId();

            return await _donationOrderService.CreateAsync(donationOrderDto, userId);
        }

        /// <summary>
        /// Updates an existing pending donation order.
        /// </summary>
        /// <remarks>
        /// This endpoint is available only for donors.
        /// A donor should only be able to update his own pending donation order.
        /// </remarks>
        /// <param name="id">The donation order unique identifier.</param>
        /// <param name="donationOrderDto">The updated donation order data.</param>
        /// <returns>The update operation result.</returns>
        /// <response code="200">Returns the update result.</response>
        /// <response code="400">The donation order cannot be updated.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not allowed to update this donation order.</response>
        /// <response code="404">Donation order was not found.</response>
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

        /// <summary>
        /// Approves a pending donation order.
        /// </summary>
        /// <remarks>
        /// This endpoint is available only for admins.
        /// When approved, the donation amount should be applied to the related donation request.
        /// </remarks>
        /// <param name="id">The donation order unique identifier.</param>
        /// <returns>The approved donation order result.</returns>
        /// <response code="200">Returns the approved donation order result.</response>
        /// <response code="400">The donation order cannot be approved.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have Admin role.</response>
        /// <response code="404">Donation order or related donation request was not found.</response>
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

        /// <summary>
        /// Rejects a pending donation order.
        /// </summary>
        /// <remarks>
        /// This endpoint is available only for admins.
        /// Rejected donation orders will not affect the related donation request amount.
        /// </remarks>
        /// <param name="id">The donation order unique identifier.</param>
        /// <returns>The rejected donation order result.</returns>
        /// <response code="200">Returns the rejected donation order result.</response>
        /// <response code="400">The donation order cannot be rejected.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have Admin role.</response>
        /// <response code="404">Donation order was not found.</response>
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

        #endregion

        #region Private Helpers

        /// <summary>
        /// Gets the authenticated user's unique identifier from the JWT claims.
        /// </summary>
        /// <returns>The authenticated user's unique identifier.</returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the user ID claim is missing or invalid.
        /// </exception>
        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User ID was not found in token.");

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedAccessException("Invalid user ID in token.");

            return parsedUserId;
        }

        #endregion
    }
}