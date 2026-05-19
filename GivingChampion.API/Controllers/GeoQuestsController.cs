using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO;
using GivingChampion.Application.DTO.GeoQuestDto;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// Handles HTTP requests for GeoQuests.
    /// </summary>
    public class GeoQuestsController : ControllerBase
    {
        private readonly IGeoQuestService _geoQuestService;
        private readonly IUserGeoQuestService _userGeoQuestService;

        /// <summary>
        /// Performs the GeoQuestsController operation.
        /// </summary>
        /// <param name="geoQuestService">Provides the geoQuestService value required by the operation.</param>
        /// <param name="userGeoQuestService">Provides the userGeoQuestService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public GeoQuestsController(IGeoQuestService geoQuestService, IUserGeoQuestService userGeoQuestService)
        {
            _geoQuestService = geoQuestService;
            _userGeoQuestService = userGeoQuestService;
        }

        [HttpGet]
        [AllowAnonymous]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var result = await _geoQuestService.GetAllAsync(pageParameters);
            if (!result.Succeeded)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _geoQuestService.GetByIdAsync(id);
            if (!result.Succeeded || result.Value is null)
                return NotFound(result.Error ?? "GeoQuest not found");

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Create([FromBody] CreateGeoQuestDto dto)
        {
            var result = await _geoQuestService.CreateAsync(dto);
            if (!result.Succeeded || result.Value is null)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGeoQuestDto dto)
        {
            var result = await _geoQuestService.UpdateAsync(id, dto);
            if (!result.Succeeded || !result.Value)
                return NotFound(result.Error ?? "GeoQuest not found");

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Removes or deactivates the specified record according to business rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _geoQuestService.SoftDeleteAsync(id);
            if (!result.Succeeded || !result.Value)
                return NotFound(result.Error ?? "GeoQuest not found");

            return NoContent();
        }

        [HttpPost("{geoQuestId:guid}/start")]
        /// <summary>
        /// Starts the requested activity for the current user.
        /// </summary>
        /// <param name="geoQuestId">Provides the geoQuestId value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> StartGeoQuest(Guid geoQuestId)
        {
            var result = await _userGeoQuestService.StartAsync(geoQuestId);
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
