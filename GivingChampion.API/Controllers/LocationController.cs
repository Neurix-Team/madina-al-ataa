using GivingChampion.Application.Interfaces.Location;
using GivingChampion.Application.DTO.Location;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/location")]
    [Produces("application/json")]
    /// <summary>
    /// Handles HTTP requests for Locations.
    /// </summary>
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        /// <summary>
        /// Performs the LocationsController operation.
        /// </summary>
        /// <param name="locationService">Provides the locationService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        [AllowAnonymous]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<List<LocationDto>>>> GetAll()
        {
            var result = await _locationService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("available")]
        [Authorize]
        /// <summary>
        /// Performs the GetAvailableForUser operation.
        /// </summary>
        /// <param name="userLevel">Provides the userLevel value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<List<LocationDto>>>> GetAvailableForUser([FromQuery] int userLevel)
        {
            var result = await _locationService.GetAvailableForUserAsync(userLevel);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<LocationDto>>> GetById(Guid id)
        {
            var result = await _locationService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result<LocationDto>>> CreateLocation([FromBody] CreateLocationDto dto)
        {
            var result = await _locationService.CreateLocationAsync(dto);
            return result.Succeeded
                ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result)
                : BadRequest(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result>> UpdateLocation(Guid id, [FromBody] UpdateLocationDto dto)
        {
            var result = await _locationService.UpdateLocationAsync(id, dto);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Removes or deactivates the specified record according to business rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<Result>> SoftDeleteLocation(Guid id)
        {
            var result = await _locationService.SoftDeleteLocationAsync(id);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}
