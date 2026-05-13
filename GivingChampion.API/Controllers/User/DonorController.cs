using AutoMapper;
using GivingChampion.Application.Interfaces.User;
using GivingChampion.Application.DTO.Donor;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/donor")]
    [Produces("application/json")]
    /// <summary>
    /// Handles HTTP requests for Donor.
    /// </summary>
    public class DonorController : ControllerBase
    {
        private readonly IDonorService _donorService;

        /// <summary>
        /// Performs the DonorController operation.
        /// </summary>
        /// <param name="donorService">Provides the donorService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public DonorController(IDonorService donorService)
        {
            _donorService = donorService;
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
            var result = await _donorService.GetMyDonorProfileAsync();
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
        /// Updates donor profile (admin)
        /// </summary>
        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(typeof(Result<DonorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<DonorDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<DonorDto>>> UpdateMyDonorProfile([FromBody] UpdateDonorDto dto)
        {
            var result = await _donorService.UpdateDonorAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Soft deletes donor profile (Admin only)
        /// </summary>
        //[HttpDelete("{userId:guid}")]
        //[Authorize(Roles = "Admin")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> SoftDeleteDonor(Guid userId)
        //{
        //    var result = await _donorService.SoftDeleteDonorAsync(userId);
        //    return result.Succeeded ? NoContent() : BadRequest(result);
        //}
    }
}
