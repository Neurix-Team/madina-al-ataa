using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO;
using GivingChampion.Application.DTO.GeoQuestDto;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GeoQuestsController : ControllerBase
    {
        private readonly IGeoQuestService _geoQuestService;
        private readonly IUserGeoQuestService _userGeoQuestService;

        public GeoQuestsController(IGeoQuestService geoQuestService, IUserGeoQuestService userGeoQuestService)
        {
            _geoQuestService = geoQuestService;
            _userGeoQuestService = userGeoQuestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var result = await _geoQuestService.GetAllAsync(pageParameters);
            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _geoQuestService.GetByIdAsync(id);
            if (!result.Succeeded || result.Value is null)
                return NotFound(result.Error ?? "GeoQuest not found");

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGeoQuestDto dto)
        {
            var result = await _geoQuestService.CreateAsync(dto);
            if (!result.Succeeded || result.Value is null)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }
        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromQuery] UpdateGeoQuestDto dto)
        {
            var result = await _geoQuestService.UpdateAsync(id, dto);
            if (!result.Succeeded || !result.Value)
                return NotFound(result.Error ?? "GeoQuest not found");

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _geoQuestService.SoftDeleteAsync(id);
            if (!result.Succeeded || !result.Value)
                return NotFound(result.Error ?? "GeoQuest not found");

            return NoContent();
        }

        [HttpPost("{geoQuestId:guid}/start")]
        public async Task<IActionResult> StartGeoQuest(Guid geoQuestId)
        {
            
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user id in token.");

            var result = await _userGeoQuestService.StartAsync(geoQuestId, userId);
            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok(new
            {
                message = "GeoQuest started successfully.",
                data = result.Value
            });
        }

       
    }
}
