using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Application.DTO.ServiceRequestDto;
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
        private readonly ILogger<ServiceRequestsController> _logger;

        public ServiceRequestsController(
            IServiceRequestService serviceRequestService,
            ILogger<ServiceRequestsController> logger)
        {
            _serviceRequestService = serviceRequestService;
            _logger = logger;
        }

        // GET: api/ServiceRequests?pageNumber=1&pageSize=20
        // GET: api/ServiceRequests?status=Approved&pageNumber=1&pageSize=20
        [Authorize(Roles = "Volunteer")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] PageParameters pageParameters,
            [FromQuery] RequestStatus? status)
        {
            try
            {
                if (status.HasValue)
                {
                    var filteredResult = await _serviceRequestService.GetByStatusAsync(
                        status.Value,
                        pageParameters);

                    return Ok(filteredResult);
                }

                var result = await _serviceRequestService.GetAllAsync(pageParameters);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service requests.");

                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving service requests.",
                    error = ex.Message
                });
            }
        }

        // GET: api/ServiceRequests/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var serviceRequest = await _serviceRequestService.GetByIdAsync(id);

            if (serviceRequest == null)
                return NotFound(new { message = "Service request not found." });

            return Ok(serviceRequest);
        }

        // GET: api/ServiceRequests/filter?status=Approved&pageNumber=1&pageSize=20
        [HttpGet("filter")]
        [Authorize(Roles = "Volunteer,Admin")]
        public async Task<IActionResult> GetByStatus(
            [FromQuery] RequestStatus status,
            [FromQuery] PageParameters pageParameters)
        {
            try
            {
                var result = await _serviceRequestService.GetByStatusAsync(
                    status,
                    pageParameters);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering service requests by status {Status}.", status);

                return StatusCode(500, new
                {
                    message = "Internal server error while filtering requests.",
                    error = ex.Message
                });
            }
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
            var updated = await _serviceRequestService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound(new { message = "Service request not found." });

            return NoContent();
        }

        // DELETE: api/ServiceRequests/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _serviceRequestService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Service request not found." });

            return NoContent();
        }
    }
}