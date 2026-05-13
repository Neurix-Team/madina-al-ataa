using GivingChampion.Application.DTO.Volunteer;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/volunteer")]
    [Produces("application/json")]
    /// <summary>
    /// Handles HTTP requests for Volunteer.
    /// </summary>
    public class VolunteerController : ControllerBase
    {
        private readonly IVolunteerService _volunteerService;

        /// <summary>
        /// Performs the VolunteerController operation.
        /// </summary>
        /// <param name="volunteerService">Provides the volunteerService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public VolunteerController(IVolunteerService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        /// <summary>
        /// Gets the volunteer profile of the currently authenticated user
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(Result<VolunteerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<VolunteerDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<VolunteerDto>>> GetMyVolunteerProfile()
        {
            var result = await _volunteerService.GetMyVolunteerProfileAsync();
            return Ok(result);
        }

        /// <summary>
        /// Admin gets volunteer profile by user ID
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<VolunteerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<VolunteerDto>>> GetVolunteerByUserId(Guid userId)
        {
            var result = await _volunteerService.GetVolunteerByUserIdAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Updates volunteer profile
        /// </summary>
        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(typeof(Result<VolunteerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<VolunteerDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<VolunteerDto>>> UpdateMyVolunteerProfile([FromBody] UpdateVolunteerDto dto)
        {
            var result = await _volunteerService.UpdateVolunteerAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Soft deletes volunteer profile (Admin only)
        /// </summary>
        //[HttpDelete("{userId:guid}")]
        //[Authorize(Roles = "Admin")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> SoftDeleteVolunteer(Guid userId)
        //{
        //    var result = await _volunteerService.SoftDeleteVolunteerAsync(userId);
        //    return result.Succeeded ? NoContent() : BadRequest(result);
        //}
    }
}
