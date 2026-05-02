using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Common.DTO.ServiceRequestDto;
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

        #endregion




        #region Get Endpoints
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

                    if (!filteredResult.IsSuccess)
                        return BadRequest(filteredResult);

                    return Ok(filteredResult);
                }

                var result = await _serviceRequestService.GetAllAsync(pageParameters);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all service requests.");

                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving service requests."
                });
            }
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
        [Authorize(Roles = "Volunteer, Admin")] 
        public async Task<IActionResult> GetByStatus([FromQuery] RequestStatus status)
        {
            try
            {
                var filteredRequests = await _serviceRequestService.GetByStatusAsync(status);

                if (filteredRequests == null || !filteredRequests.Any())
                {
                    return NotFound(new { Message = $"No service requests found with status: {status}" });
                }

                return Ok(filteredRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering service requests by status {Status}.", status);
                return StatusCode(500, new { Message = "Internal server error while filtering requests." });
            }
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