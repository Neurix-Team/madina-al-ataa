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
        [Authorize(Roles = "Donor")]
        public async Task<ActionResult<PagedList<DonationRequestDto>>> GetMyDonationRequests([FromQuery] PageParameters pageParameters)
        {
            var donationRequests = await _donationRequestService.GetMyRequestsAsync(pageParameters);
            return Ok(donationRequests);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Donor,Admin")]
        public async Task<ActionResult<DonationRequestDto>> GetById(Guid id)
        {
            var donationRequest = await _donationRequestService.GetByIdAsync(id, User.IsInRole("Admin"));

            if (donationRequest == null)
                throw new KeyNotFoundException($"Donation request with ID {id} not found.");

            return Ok(donationRequest);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateDonationRequestDto dto)
        {
            await _donationRequestService.UpdateAsync(id, dto, User.IsInRole("Admin"));
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _donationRequestService.SoftDeleteAsync(id, User.IsInRole("Admin"));
            return NoContent();
        }
    }
}
