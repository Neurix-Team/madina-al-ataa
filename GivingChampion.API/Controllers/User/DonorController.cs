using AutoMapper;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Common.DTO.Donor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/donor")]
    [Produces("application/json")]
    public class DonorController : ControllerBase
    {
        private readonly IDonorService _donorService;

        public DonorController(IDonorService donorService)
        {
            _donorService = donorService;
        }

        /// <summary>
        /// Creates a donor profile for the currently authenticated user
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Result<DonorDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result<DonorDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<DonorDto>>> CreateDonor([FromBody] CreateDonorDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            var result = await _donorService.CreateDonorAsync(dto, userId);
            return result.Succeeded
                ? CreatedAtAction(nameof(GetMyDonorProfile), result)
                : BadRequest(result);
        }

        /// <summary>
        /// Gets the donor profile of the currently authenticated user
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(Result<DonorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<DonorDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<DonorDto>>> GetMyDonorProfile()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            var result = await _donorService.GetMyDonorProfileAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Admin gets donor profile by user ID
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<DonorDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<DonorDto>>> GetDonorByUserId(Guid userId)
        {
            var result = await _donorService.GetDonorByUserIdAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Updates donor profile (parent or admin)
        /// </summary>
        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(typeof(Result<DonorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<DonorDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<DonorDto>>> UpdateMyDonorProfile([FromBody] UpdateDonorDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            var result = await _donorService.UpdateDonorAsync(dto, userId);
            return Ok(result);
        }

        /// <summary>
        /// Soft deletes donor profile (Admin only)
        /// </summary>
        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SoftDeleteDonor(Guid userId)
        {
            var result = await _donorService.SoftDeleteDonorAsync(userId);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}