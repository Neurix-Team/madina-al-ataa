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
    /// <summary>
    /// Handles HTTP requests for ServiceRequests.
    /// </summary>
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        /// <summary>
        /// Performs the ServiceRequestsController operation.
        /// </summary>
        /// <param name="serviceRequestService">Provides the serviceRequestService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public ServiceRequestsController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var result = await _serviceRequestService.GetAllAsync(pageParameters);

            return Ok(result);
        }
        [HttpGet("approved")]
        [AllowAnonymous]
        /// <summary>
        /// Retrieves approved records that are visible to the requesting role.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetApproved([FromQuery] PageParameters pageParameters)
        {
            var result = await _serviceRequestService.GetApprovedRequestsAsync(pageParameters);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetById(Guid id)
        {
            var serviceRequest = await _serviceRequestService.GetByIdAsync(id);
            return Ok(serviceRequest);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
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
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
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
        /// <summary>
        /// Removes or deactivates the specified record according to business rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _serviceRequestService.DeleteAsync(id))
                throw new NotFoundException("Service request not found.");

            return NoContent();
        }
    }
}
