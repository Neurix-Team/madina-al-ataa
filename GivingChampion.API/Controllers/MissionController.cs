using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO.Mission;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/mission")]
    [Produces("application/json")]
    /// <summary>
    /// Handles HTTP requests for Missions.
    /// </summary>
    public class MissionsController : ControllerBase
    {
        private readonly IMissionService _missionService;

        /// <summary>
        /// Performs the MissionsController operation.
        /// </summary>
        /// <param name="missionService">Provides the missionService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public MissionsController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        // Admin: Get All Active Missions
        [HttpGet]
        [AllowAnonymous]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<PagedList<MissionDto>>>> GetAllOpenAsync([FromQuery] PageParameters pageParameters)
        {
            var result = await _missionService.GetAllOpenAsync(pageParameters);
            return Ok(result);
        }

        // User: Get Available Missions
        [HttpGet("available")]
        [Authorize]
        /// <summary>
        /// Performs the GetAvailableForUser operation.
        /// </summary>
        /// <param name="userLevel">Provides the userLevel value required by the operation.</param>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<PagedList<MissionDto>>>> GetAvailableForUser([FromQuery] int userLevel, [FromQuery] PageParameters pageParameters)
        {
            var result = await _missionService.GetAvailableForUserAsync(userLevel, pageParameters);
            return Ok(result);
        }

        // Get Mission by ID (Admin & Volunteer)
        [HttpGet("{id:guid}")]
        [Authorize]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<MissionDto>>> GetById(Guid id)
        {
            var result = await _missionService.GetByIdAsync(id);
            return Ok(result);
        }

        // Admin: Create Mission
        [HttpPost]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<MissionDto>>> CreateMission([FromBody] CreateMissionDto dto)
        {
            var result = await _missionService.CreateMissionAsync(dto);
            return result.Succeeded
                ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result)
                : BadRequest(result);
        }

        // Admin: Update Mission
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result>> UpdateMission(Guid id, [FromBody] UpdateMissionDto dto)
        {
            var result = await _missionService.UpdateMissionAsync(id, dto);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // Admin: Soft Delete Mission
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Removes or deactivates the specified record according to business rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result>> SoftDeleteMission(Guid id)
        {
            var result = await _missionService.SoftDeleteMissionAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}
