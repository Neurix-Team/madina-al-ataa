using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Mission;
using GivingChampion.Application.DTO.Mission;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/user-mission")]
    [Produces("application/json")]
    /// <summary>
    /// Handles HTTP requests for UserMissions.
    /// </summary>
    public class UserMissionsController : ControllerBase
    {
        private readonly IUserMissionService _userMissionService;

        /// <summary>
        /// Performs the UserMissionsController operation.
        /// </summary>
        /// <param name="userMissionService">Provides the userMissionService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public UserMissionsController(IUserMissionService userMissionService)
        {
            _userMissionService = userMissionService;
        }

        // Volunteer: Start Mission
        [HttpPost]
        [Authorize]
        /// <summary>
        /// Starts the requested activity for the current user.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<UserMissionDto>>> StartMission([FromBody] StartMissionDto dto)
        {
            var result = await _userMissionService.StartMissionAsync(dto);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // Volunteer: Update Mission Progress
        [HttpPut("{id:guid}/progress")]
        [Authorize]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<UserMissionDto>>> UpdateMissionProgress(Guid id, [FromBody] UpdateProgressDto dto)
        {
            var result = await _userMissionService.UpdateProgressAsync(id, dto);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // Volunteer: Get Active Missions
        [HttpGet("active")]
        [Authorize]
        /// <summary>
        /// Performs the GetActiveMissions operation.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<PagedList<UserMissionDto>>>> GetActiveMissions([FromQuery] PageParameters pageParameters)
        {
            var result = await _userMissionService.GetMyActiveMissionsAsync(pageParameters);
            return Ok(result);
        }

        // Volunteer: Get Completed Missions
        [HttpGet("completed")]
        [Authorize]
        /// <summary>
        /// Performs the GetCompletedMissions operation.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<PagedList<UserMissionDto>>>> GetCompletedMissions([FromQuery] PageParameters pageParameters)
        {
            var result = await _userMissionService.GetMyCompletedMissionsAsync(pageParameters);
            return Ok(result);
        }

        // Volunteer: Get Mission by ID
        [HttpGet("{id:guid}")]
        [Authorize]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<UserMissionDto>>> GetById(Guid id)
        {
            var result = await _userMissionService.GetByIdAsync(id);
            return Ok(result);
        }
    }
}
