using GivingChampion.Application.DTO.ServiceRequestDto;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestsController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpGet]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> GetAll(
            [FromQuery] PageParameters pageParameters,
            [FromQuery] RequestStatus? status)
        {
            var result = status.HasValue
                ? await _serviceRequestService.GetByStatusAsync(status.Value, pageParameters)
                : await _serviceRequestService.GetAllAsync(pageParameters);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var serviceRequest = await _serviceRequestService.GetByIdAsync(id)
                ?? throw new NotFoundException("Service request not found.");

            return Ok(serviceRequest);
        }

        [HttpGet("filter")]
        [Authorize(Roles = "Volunteer,Admin")]
        public async Task<IActionResult> GetByStatus(
            [FromQuery] RequestStatus status,
            [FromQuery] PageParameters pageParameters)
        {
            var result = await _serviceRequestService.GetByStatusAsync(
                status,
                pageParameters);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequestDto dto)
        {
            var createdServiceRequest = await _serviceRequestService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdServiceRequest.Id },
                createdServiceRequest
            );
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateServiceRequestDto dto)
        {
            if (!await _serviceRequestService.UpdateAsync(id, dto))
                throw new NotFoundException("Service request not found.");

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _serviceRequestService.DeleteAsync(id))
                throw new NotFoundException("Service request not found.");

            return NoContent();
        }
    }
}