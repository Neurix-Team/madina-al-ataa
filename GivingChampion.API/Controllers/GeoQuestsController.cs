using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO;
using GivingChampion.Common.DTO.GeoQuestDto;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = "Admin")]
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
        // POST api/geoquests/{geoQuestId}/start

        [HttpPost("{geoQuestId}/start")]
        public async Task<IActionResult> StartGeoQuest(Guid geoQuestId, [FromBody] StartGeoQuestDto dto)
        {
            // Get the UserGeoQuest by ID
            var userGeoQuestResult = await _userGeoQuestService.GetByIdAsync(dto.UserGeoQuestId);

            if (userGeoQuestResult.IsSuccess)
            {
                var userGeoQuest = userGeoQuestResult.Value;

                // Start the GeoQuest (Mark as in progress)
                userGeoQuest.GeoQuestId = geoQuestId; // Associate the GeoQuest with the UserGeoQuest
                userGeoQuest.StartedAt = DateTime.UtcNow; // Set the start time

                // Update the UserGeoQuest record
                var updateResult = await _userGeoQuestService.StartAsync(userGeoQuest.Id, dto);

                if (updateResult.IsSuccess)
                {
                    return Ok("GeoQuest started successfully");
                }

                return BadRequest("Failed to update UserGeoQuest");
            }

            return NotFound("UserGeoQuest not found");
        }


        [HttpPost("{geoQuestId}/verify-location")]
        public async Task<IActionResult> VerifyLocation(Guid geoQuestId, [FromBody] VerifyLocationDto dto)
        {
            // Get the UserGeoQuest DTO by UserGeoQuestId
            var userGeoQuestResult = await _userGeoQuestService.GetByIdAsync(dto.UserGeoQuestId);

            // Check if the userGeoQuest was successfully retrieved
            if (userGeoQuestResult.IsFailure)
            {
                return NotFound("UserGeoQuest not found");
            }

            // Extract the actual DTO value
            var userGeoQuest = userGeoQuestResult.Value;

            // Verify if the user's location matches the required location for this GeoQuest
            if (dto.Latitude == userGeoQuest.LocationLatitude &&
                dto.Longitude == userGeoQuest.LocationLongitude)
            {
                userGeoQuest.IsLocationVerified = true;

                // Update the UserGeoQuest record with the new location verification status
                await _userGeoQuestService.UpdateAsyncVerification(userGeoQuest.Id, dto, isSuccess: true); 

                return Ok("Location verified successfully");
            }

            return BadRequest("Location is incorrect");
        }
    }



    }
