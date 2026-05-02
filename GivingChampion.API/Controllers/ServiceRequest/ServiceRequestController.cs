using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Application.DTO.ServiceRequestDto;
using GivingChampion.Common.Enums;
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

        // GET: api/ServiceRequests
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] RequestStatus? status)
        {
            if (status.HasValue)
            {
                var filtered = await _serviceRequestService.GetByStatusAsync(status.Value);
                return Ok(filtered);
            }

            var serviceRequests = await _serviceRequestService.GetAllAsync();

            return Ok(serviceRequests);
        }

        // GET: api/ServiceRequests/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var serviceRequest = await _serviceRequestService.GetByIdAsync(id);

            return Ok(serviceRequest);
        }

        // POST: api/ServiceRequests
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

        // PUT: api/ServiceRequests/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceRequestDto dto)
        {
            await _serviceRequestService.UpdateAsync(id, dto);

            return NoContent();
        }

        // GET: api/ServiceRequests/filter?status=Approved
        [HttpGet("filter")]
        public async Task<IActionResult> GetByStatus([FromQuery] RequestStatus status)
        {
            var filteredRequests = await _serviceRequestService.GetByStatusAsync(status);

            if (!filteredRequests.Any())
            {
                return NotFound(new
                {
                    Message = $"No service requests found with status: {status}"
                });
            }

            return Ok(filteredRequests);
        }

        // DELETE: api/ServiceRequests/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _serviceRequestService.DeleteAsync(id);

            return NoContent();
        }
    }
}