using GivingChampion.Application.Interfaces.ServiceRequestService;
using GivingChampion.Common.DTO.ServiceRequestDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        #region Fields

        // Service layer used to handle business logic for service requests
        private readonly IServiceRequestService _serviceRequestService;

        // Logger used to log unexpected errors
        private readonly ILogger<ServiceRequestsController> _logger;

        #endregion

        #region Constructor

        public ServiceRequestsController(
            IServiceRequestService serviceRequestService,
            ILogger<ServiceRequestsController> logger)
        {
            _serviceRequestService = serviceRequestService;
            _logger = logger;
        }

        #endregion

        #region Get Endpoints

        // GET: api/ServiceRequests
        // Returns all service requests that are not soft deleted
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var serviceRequests = await _serviceRequestService.GetAllAsync();

                return Ok(serviceRequests);
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
        // Returns a single service request by id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid service request id."
                    });
                }

                var serviceRequest = await _serviceRequestService.GetByIdAsync(id);

                if (serviceRequest == null)
                {
                    return NotFound(new
                    {
                        Message = "Service request not found."
                    });
                }

                return Ok(serviceRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while getting service request with id {ServiceRequestId}.",
                    id
                );

                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving the service request."
                });
            }
        }

        // GET: api/ServiceRequests/pending
        // Returns all pending service requests
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var serviceRequests = await _serviceRequestService.GetPendingAsync();

                return Ok(serviceRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting pending service requests.");

                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving pending service requests."
                });
            }
        }

        // GET: api/ServiceRequests/partner/{partnerId}
        // Returns all service requests related to a specific partner
        [HttpGet("partner/{partnerId:guid}")]
        public async Task<IActionResult> GetByPartnerId(Guid partnerId)
        {
            try
            {
                if (partnerId == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid partner id."
                    });
                }

                var serviceRequests = await _serviceRequestService.GetByPartnerIdAsync(partnerId);

                return Ok(serviceRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while getting service requests for partner {PartnerId}.",
                    partnerId
                );

                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving partner service requests."
                });
            }
        }

        #endregion

        #region Create Endpoint

        // POST: api/ServiceRequests
        // Creates a new service request
        // Only Admin users are allowed to create service requests
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequestDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        Message = "Request body cannot be null."
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdServiceRequest = await _serviceRequestService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdServiceRequest.Id },
                    createdServiceRequest
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating service request.");

                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while creating the service request."
                });
            }
        }

        #endregion

        #region Update Endpoint

        // PUT: api/ServiceRequests/{id}
        // Updates an existing service request
        // Only Admin users are allowed to update service requests
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceRequestDto dto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid service request id."
                    });
                }

                if (dto == null)
                {
                    return BadRequest(new
                    {
                        Message = "Request body cannot be null."
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var isUpdated = await _serviceRequestService.UpdateAsync(id, dto);

                if (!isUpdated)
                {
                    return NotFound(new
                    {
                        Message = "Service request not found."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while updating service request with id {ServiceRequestId}.",
                    id
                );

                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while updating the service request."
                });
            }
        }

        #endregion

        #region Delete Endpoint

        // DELETE: api/ServiceRequests/{id}
        // Soft deletes an existing service request
        // Only Admin users are allowed to delete service requests
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid service request id."
                    });
                }

                var isDeleted = await _serviceRequestService.DeleteAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new
                    {
                        Message = "Service request not found."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while deleting service request with id {ServiceRequestId}.",
                    id
                );

                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while deleting the service request."
                });
            }
        }

        #endregion
    }
}