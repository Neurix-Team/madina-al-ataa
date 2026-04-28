using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO.DonationRequest;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GivingChampion.API.Controllers
{
    [Route("api/donation-requests")]
    [ApiController]
    [Authorize]
    public class DonationRequestsController : ControllerBase
    {
        private readonly IDonationRequestService _donationRequestService;

        public DonationRequestsController(IDonationRequestService donationRequestService)
        {
            _donationRequestService = donationRequestService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedList<DonationRequestDto>>> GetAll([FromQuery] PageParameters pageParameters)
        {
            var donationRequests = await _donationRequestService.GetAllAsync(pageParameters);

            return Ok(donationRequests);
        }

        [HttpGet("approved")]
        [Authorize(Roles = "Donor,Admin")]
        public async Task<ActionResult<PagedList<DonationRequestDto>>> GetApprovedDonationRequests([FromQuery] PageParameters pageParameters)
        {
            var donationRequests = await _donationRequestService.GetApprovedAsync(pageParameters);

            return Ok(donationRequests);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<PagedList<DonationRequestDto>>> GetMyDonationRequests([FromQuery] PageParameters pageParameters)
        {
            var userId = GetCurrentUserId();

            var donationRequests = await _donationRequestService.GetMyRequestsAsync(userId, pageParameters);

            return Ok(donationRequests);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Donor,Parent,Admin")]
        public async Task<ActionResult<DonationRequestDto>> GetById(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var donationRequest = await _donationRequestService.GetByIdAsync(id, userId, isAdmin);

            if (donationRequest == null)
                throw new KeyNotFoundException($"Donation request with ID {id} not found.");

            return Ok(donationRequest);
        }

        [HttpPost]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<ActionResult<DonationRequestDto>> Create([FromBody] CreateDonationRequestDto dto)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var createdDonationRequest = await _donationRequestService.AddAsync(dto, userId, isAdmin);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdDonationRequest.Id },
                createdDonationRequest
            );
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateDonationRequestDto dto)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            await _donationRequestService.UpdateAsync(id, dto, userId, isAdmin);

            return NoContent();
        }

        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Approve(Guid id)
        {
            await _donationRequestService.ApproveAsync(id);

            return NoContent();
        }

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Reject(Guid id)
        {
            await _donationRequestService.RejectAsync(id);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            await _donationRequestService.SoftDeleteAsync(id, userId, isAdmin);

            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User ID was not found in token.");

            return Guid.Parse(userId);
        }
    }
}