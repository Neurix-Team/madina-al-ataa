using GivingChampion.Application.DTO.Partner;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.Partner;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// Handles HTTP requests for Partners.
    /// </summary>
    public class PartnersController : ControllerBase
    {
        private readonly IPartnerService _partnerService;

        /// <summary>
        /// Performs the PartnersController operation.
        /// </summary>
        /// <param name="partnerService">Provides the partnerService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public PartnersController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [HttpGet("{id:guid}")]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetById(Guid id)
        {
            var partner = await _partnerService.GetByIdAsync(id)
                ?? throw new NotFoundException($"Partner with ID {id} not found.");

            return Ok(partner);
        }

        [HttpGet]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var result = await _partnerService.GetAllAsync(pageParameters);

            return result.Succeeded
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Create([FromBody] CreatePartnerDto dto)
        {
            var partner = await _partnerService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = partner.Id },
                partner
            );
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartnerDto dto)
        {
            var partner = await _partnerService.UpdateAsync(id, dto)
                ?? throw new NotFoundException($"Partner with ID {id} not found.");

            return Ok(partner);
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
            if (!await _partnerService.DeleteAsync(id))
                throw new NotFoundException($"Partner with ID {id} not found for deletion.");

            return NoContent();
        }
    }
}
