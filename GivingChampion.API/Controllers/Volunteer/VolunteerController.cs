using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Volunteer
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VolunteerController : ControllerBase
    {
        private readonly IVolunteerService _volunteerService;

        public VolunteerController(IVolunteerService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var volunteer = await _volunteerService.GetByIdAsync(id)
                ?? throw new NotFoundException($"Volunteer with ID {id} not found.");

            return Ok(volunteer);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var result = await _volunteerService.GetAllAsync(pageParameters);

            return result.Succeeded
                ? Ok(result)
                : BadRequest(result);
        }
    }
}