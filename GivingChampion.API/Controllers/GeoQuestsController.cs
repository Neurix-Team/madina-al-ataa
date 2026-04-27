using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO;
using GivingChampion.Common.DTO.GeoQuestDto;
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

        // GET api/geoquests
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var geoQuests = await _geoQuestService.GetAllAsync(pageParameters);
            return Ok(geoQuests);
        }

        // GET api/geoquests/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var geoQuest = await _geoQuestService.GetByIdAsync(id);
            if (geoQuest == null)
                return NotFound("GeoQuest not found");

            return Ok(geoQuest);
        }

        // POST api/geoquests
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGeoQuestDto dto)
        {
            var created = await _geoQuestService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Value.Id }, created);
        }

        // PUT api/geoquests/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGeoQuestDto dto)
        {
            var updated = await _geoQuestService.UpdateAsync(id, dto);
            if (!updated.Value) return NotFound("GeoQuest not found");
            return NoContent();
        }

        // DELETE api/geoquests/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _geoQuestService.SoftDeleteAsync(id);
            if (!deleted.Value) return NotFound("GeoQuest not found");
            return NoContent();
        }

        [Authorize]
        [HttpPost("{geoQuestId}/start")]
        public async Task<IActionResult> StartGeoQuest(Guid geoQuestId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("User is not authenticated.");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user id in token.");

            var result = await _userGeoQuestService.StartAsync(geoQuestId, userId);

            if (!result.IsSuccess)
                return BadRequest(new
                {
                    message = result.Error
                });

            return Ok(new
            {
                message = "GeoQuest started successfully.",
                data = result.Value
            });
        }

        [Authorize]
        [HttpPost("{geoQuestId}/verify-location")]
        public async Task<IActionResult> VerifyLocation(Guid geoQuestId, [FromBody] VerifyLocationDto dto)
        {
            if (dto == null)
                return BadRequest("Request body is required.");

            if (dto.UserGeoQuestId == Guid.Empty)
                return BadRequest("UserGeoQuestId is required.");

            var userGeoQuestResult = await _userGeoQuestService.GetByIdAsync(dto.UserGeoQuestId);

            if (userGeoQuestResult.IsFailure || userGeoQuestResult.Value == null)
                return NotFound("UserGeoQuest not found.");

            var userGeoQuest = userGeoQuestResult.Value;

            if (userGeoQuest.GeoQuestId != geoQuestId)
                return BadRequest("This UserGeoQuest does not belong to the provided GeoQuest.");

            var result = await _userGeoQuestService.UpdateAsyncVerification(
                dto.UserGeoQuestId,
                dto,
                isSuccess: true);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }


}
