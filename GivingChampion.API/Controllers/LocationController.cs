using GivingChampion.Application.Interfaces.Location;
using GivingChampion.Common.DTO.Location;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/location")]
    [Produces("application/json")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Result<List<LocationDto>>>> GetAll()
        {
            var result = await _locationService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("available")]
        [Authorize]
        public async Task<ActionResult<Result<List<LocationDto>>>> GetAvailableForUser([FromQuery] int userLevel)
        {
            var result = await _locationService.GetAvailableForUserAsync(userLevel);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<Result<LocationDto>>> GetById(Guid id)
        {
            var result = await _locationService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<LocationDto>>> CreateLocation([FromBody] CreateLocationDto dto)
        {
            var result = await _locationService.CreateLocationAsync(dto);
            return result.Succeeded
                ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result)
                : BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> UpdateLocation(Guid id, [FromBody] UpdateLocationDto dto)
        {
            var result = await _locationService.UpdateLocationAsync(id, dto);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> SoftDeleteLocation(Guid id)
        {
            var result = await _locationService.SoftDeleteLocationAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}