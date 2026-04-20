using GivingChampion.Application.Interfaces.Mission;
using GivingChampion.Common.DTO.Mission;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/user-mission")]
    [Produces("application/json")]
    [Authorize]
    public class UsermissionsController : ControllerBase
    {
        private readonly IUserMissionService _userMissionService;

        public UsermissionsController(IUserMissionService userMissionService)
        {
            _userMissionService = userMissionService;
        }

        #region User Actions

        /// <summary>
        /// Starts a new mission for the currently authenticated user
        /// </summary>
        [HttpPost("start")]
        [Authorize]
        [ProducesResponseType(typeof(Result<UserMissionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result<UserMissionDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<UserMissionDto>>> StartMission([FromBody] StartMissionDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            var result = await _userMissionService.StartMissionAsync(dto, userId);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Updates progress on a user's mission
        /// </summary>
        [HttpPut("{userMissionId:guid}/progress")]
        [Authorize]
        [ProducesResponseType(typeof(Result<UserMissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<UserMissionDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<UserMissionDto>>> UpdateProgress(
            Guid userMissionId,
            [FromBody] UpdateProgressDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            var result = await _userMissionService.UpdateProgressAsync(userMissionId, dto, userId);
            return Ok(result);
        }

        /// <summary>
        /// Gets all active (in-progress) missions for the current user with pagination
        /// </summary>
        [HttpGet("active")]
        [Authorize]
        [ProducesResponseType(typeof(Result<PagedList<UserMissionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<PagedList<UserMissionDto>>>> GetMyActiveMissions(
            [FromQuery] PageParameters pageParameters)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            var result = await _userMissionService.GetMyActiveMissionsAsync(pageParameters, userId);
            return Ok(result);
        }

        /// <summary>
        /// Gets all completed missions for the current user with pagination
        /// </summary>
        [HttpGet("completed")]
        [Authorize]
        [ProducesResponseType(typeof(Result<PagedList<UserMissionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<PagedList<UserMissionDto>>>> GetMyCompletedMissions(
            [FromQuery] PageParameters pageParameters)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            var result = await _userMissionService.GetMyCompletedMissionsAsync(pageParameters, userId);
            return Ok(result);
        }

        /// <summary>
        /// Gets a specific user mission by ID
        /// </summary>
        [HttpGet("{userMissionId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(Result<UserMissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<UserMissionDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<UserMissionDto>>> GetById(Guid userMissionId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            var result = await _userMissionService.GetByIdAsync(userMissionId, userId);
            return Ok(result);
        }

        #endregion

        #region Admin Actions

        /// <summary>
        /// Admin gets all missions for a specific user (with pagination)
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Result<PagedList<UserMissionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<PagedList<UserMissionDto>>>> GetUserMissions(
            Guid userId,
            [FromQuery] PageParameters pageParameters)
        {
            var result = await _userMissionService.GetAllUserMissionsAsync(pageParameters, userId);
            return Ok(result);
        }

        #endregion
    }
}