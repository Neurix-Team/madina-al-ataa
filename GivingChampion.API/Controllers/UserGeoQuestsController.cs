using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserGeoQuestsController : ControllerBase
    {
        private readonly IUserGeoQuestService _userGeoQuestService;

        public UserGeoQuestsController(IUserGeoQuestService userGeoQuestService)
        {
            _userGeoQuestService = userGeoQuestService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] Guid userId, [FromQuery] PageParameters pageParameters)
        {
            var userGeoQuests = await _userGeoQuestService.GetAllAsync(userId, pageParameters);
            return Ok(userGeoQuests);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userGeoQuest = await _userGeoQuestService.GetByIdAsync(id);
            if (userGeoQuest == null || !userGeoQuest.Succeeded)
                return NotFound("UserGeoQuest not found");

            return Ok(userGeoQuest);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserGeoQuestDto dto)
        {
            var updated = await _userGeoQuestService.UpdateAsync(id, dto);
            if (!updated.Value) return NotFound("UserGeoQuest not found");
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userGeoQuestService.SoftDeleteAsync(id);
            if (!deleted.Value) return NotFound("UserGeoQuest not found");
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/status")]
        public async Task<IActionResult> CheckGeoQuestStatus(Guid id)
        {
            var result = await _userGeoQuestService.CheckGeoQuestStatus(id);

            if (!result.Succeeded)
            {
                return BadRequest("An error occurred while checking the GeoQuest status.");
            }

            return Ok(result.Value);
        }
    }
  }
