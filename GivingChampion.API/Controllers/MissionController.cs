using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO.Mission;
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
    public class MissionsController : ControllerBase
    {
        private readonly IMissionService _missionService;

        public MissionsController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        // Admin: Get All Active Missions
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<PagedList<MissionDto>>>> GetAllActive([FromQuery] PageParameters pageParameters)
        {
            var result = await _missionService.GetAllActiveAsync(pageParameters);
            return Ok(result);
        }

        // User: Get Available Missions
        [HttpGet("available")]
        [Authorize]
        public async Task<ActionResult<Result<PagedList<MissionDto>>>> GetAvailableForUser([FromQuery] int userLevel, [FromQuery] PageParameters pageParameters)
        {
            var result = await _missionService.GetAvailableForUserAsync(userLevel, pageParameters);
            return Ok(result);
        }

        // Get Mission by ID (Admin & Volunteer)
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<Result<MissionDto>>> GetById(Guid id)
        {
            var result = await _missionService.GetByIdAsync(id);
            return Ok(result);
        }

        // Admin: Create Mission
        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        public async Task<ActionResult<Result>> UpdateMission(Guid id, [FromBody] UpdateMissionDto dto)
        {
            var result = await _missionService.UpdateMissionAsync(id, dto);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // Admin: Soft Delete Mission
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> SoftDeleteMission(Guid id)
        {
            var result = await _missionService.SoftDeleteMissionAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}