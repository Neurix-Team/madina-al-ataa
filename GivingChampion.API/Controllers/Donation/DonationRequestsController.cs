using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO.DonationRequest;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Route("api/donation-requests")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// Handles HTTP requests for DonationRequests.
    /// </summary>
    public class DonationRequestsController : ControllerBase
    {
        private readonly IDonationRequestService _donationRequestService;

        /// <summary>
        /// Performs the DonationRequestsController operation.
        /// </summary>
        /// <param name="donationRequestService">Provides the donationRequestService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public DonationRequestsController(IDonationRequestService donationRequestService)
        {
            _donationRequestService = donationRequestService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<PagedList<DonationRequestDto>>> GetAll([FromQuery] PageParameters pageParameters)
        {
            var donationRequests = await _donationRequestService.GetAllAsync(pageParameters);
            return Ok(donationRequests);
        }

        [HttpGet("approved")]
        [AllowAnonymous]
        /// <summary>
        /// Retrieves approved records that are visible to the requesting role.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<PagedList<DonationRequestDto>>> GetApprovedDonationRequests([FromQuery] PageParameters pageParameters)
        {
            var donationRequests = await _donationRequestService.GetApprovedAsync(pageParameters);
            return Ok(donationRequests);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Donor")]
        /// <summary>
        /// Retrieves records owned by the current authenticated user.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<PagedList<DonationRequestDto>>> GetMyDonationRequests([FromQuery] PageParameters pageParameters)
        {
            var donationRequests = await _donationRequestService.GetMyRequestsAsync(pageParameters);
            return Ok(donationRequests);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<DonationRequestDto>> GetById(Guid id)
        {
            var donationRequest = await _donationRequestService.GetByIdAsync(id, User.IsInRole("Admin"));

            if (donationRequest == null)
                throw new KeyNotFoundException($"Donation request with ID {id} not found.");

            return Ok(donationRequest);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<DonationRequestDto>> Create([FromBody] CreateDonationRequestDto dto)
        {
            var createdDonationRequest = await _donationRequestService.AddAsync(dto, User.IsInRole("Admin"));

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdDonationRequest.Id },
                createdDonationRequest
            );
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateDonationRequestDto dto)
        {
            await _donationRequestService.UpdateAsync(id, dto, User.IsInRole("Admin"));
            return NoContent();
        }

        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Approves the specified record for the next workflow step.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult> Approve(Guid id)
        {
            await _donationRequestService.ApproveAsync(id);
            return NoContent();
        }

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Rejects the specified record according to administrative rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult> Reject(Guid id)
        {
            await _donationRequestService.RejectAsync(id);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Removes or deactivates the specified record according to business rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult> Delete(Guid id)
        {
            await _donationRequestService.SoftDeleteAsync(id, User.IsInRole("Admin"));
            return NoContent();
        }
    }
}
