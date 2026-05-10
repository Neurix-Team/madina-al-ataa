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
    public class PartnersController : ControllerBase
    {
        private readonly IPartnerService _partnerService;

        public PartnersController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var partner = await _partnerService.GetByIdAsync(id)
                ?? throw new NotFoundException($"Partner with ID {id} not found.");

            return Ok(partner);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var result = await _partnerService.GetAllAsync(pageParameters);

            return result.Succeeded
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartnerDto dto)
        {
            var partner = await _partnerService.UpdateAsync(id, dto)
                ?? throw new NotFoundException($"Partner with ID {id} not found.");

            return Ok(partner);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _partnerService.DeleteAsync(id))
                throw new NotFoundException($"Partner with ID {id} not found for deletion.");

            return NoContent();
        }
    }
}