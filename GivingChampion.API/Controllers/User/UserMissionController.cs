using GivingChampion.Application.Interfaces;
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
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/user-mission")]
    [Produces("application/json")]
    public class UserMissionsController : ControllerBase
    {
        private readonly IUserMissionService _userMissionService;

        public UserMissionsController(IUserMissionService userMissionService)
        {
            _userMissionService = userMissionService;
        }

        // Volunteer: Start Mission
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Result<UserMissionDto>>> StartMission([FromBody] StartMissionDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _userMissionService.StartMissionAsync(dto, userId);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // Volunteer: Update Mission Progress
        [HttpPut("{id:guid}/progress")]
        [Authorize]
        public async Task<ActionResult<Result<UserMissionDto>>> UpdateMissionProgress(Guid id, [FromBody] UpdateProgressDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _userMissionService.UpdateProgressAsync(id, dto, userId);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // Volunteer: Get Active Missions
        [HttpGet("active")]
        [Authorize]
        public async Task<ActionResult<Result<PagedList<UserMissionDto>>>> GetActiveMissions([FromQuery] PageParameters pageParameters)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _userMissionService.GetMyActiveMissionsAsync(pageParameters, userId);
            return Ok(result);
        }

        // Volunteer: Get Completed Missions
        [HttpGet("completed")]
        [Authorize]
        public async Task<ActionResult<Result<PagedList<UserMissionDto>>>> GetCompletedMissions([FromQuery] PageParameters pageParameters)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _userMissionService.GetMyCompletedMissionsAsync(pageParameters, userId);
            return Ok(result);
        }

        // Volunteer: Get Mission by ID
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<Result<UserMissionDto>>> GetById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _userMissionService.GetByIdAsync(id, userId);
            return Ok(result);
        }
    }
}