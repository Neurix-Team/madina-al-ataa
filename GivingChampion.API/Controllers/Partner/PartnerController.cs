using GivingChampion.Application.Interfaces.Partner;
using GivingChampion.Application.DTO.Partner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using GivingChampion.Common.Pagination;

namespace GivingChampion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Ensure that only Admin can access these endpoints
    public class PartnersController : ControllerBase
    {
        #region Fields

        private readonly IPartnerService _partnerService;
        private readonly ILogger<PartnersController> _logger;

        #endregion

        #region Constructor

        // Constructor to initialize PartnerService and Logger
        public PartnersController(IPartnerService partnerService, ILogger<PartnersController> logger)
        {
            _partnerService = partnerService;
            _logger = logger;
        }

        #endregion

        #region Query Methods

        #region GetById
        /// <summary>
        /// Retrieves a partner by its unique identifier (ID).
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var partner = await _partnerService.GetByIdAsync(id);

                // If partner is not found, return 404 Not Found
                if (partner == null)
                {
                    return NotFound($"Partner with ID {id} not found.");
                }

                // Return the partner data if found
                return Ok(partner);
            }
            catch (Exception ex)
            {
                // Log error and return 500 Internal Server Error
                _logger.LogError(ex, $"Error occurred while fetching partner with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region GetALLPartner
        /// <summary>
        /// Retrieves all partners.
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            try
            {
                var result = await _partnerService.GetAllAsync(pageParameters);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all partners.");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #endregion


        #region Command Methods

        #region CreatePartner
        /// <summary>
        /// Creates a new partner.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePartnerDto dto)
        {
            try
            {
                var createdPartner = await _partnerService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdPartner.Id }, createdPartner);
            }
            catch (Exception ex)
            {
                // Log error and return 500 Internal Server Error
                _logger.LogError(ex, "Error occurred while creating the partner.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region UpdatePartner
        /// <summary>
        /// Updates an existing partner by its ID.
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartnerDto dto)
        {
            try
            {
                var updatedPartner = await _partnerService.UpdateAsync(id, dto);

                // If partner not found, return 404 Not Found
                if (updatedPartner == null)
                {
                    return NotFound($"Partner with ID {id} not found.");
                }

                return Ok(updatedPartner);
            }
            catch (Exception ex)
            {
                // Log error and return 500 Internal Server Error
                _logger.LogError(ex, $"Error occurred while updating partner with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

        #region DeletePartner
        /// <summary>
        /// Soft deletes a partner by its ID.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _partnerService.DeleteAsync(id);

                // If partner not found, return 404 Not Found
                if (!result)
                {
                    _logger.LogWarning($"Partner with ID {id} not found for deletion.");
                    return NotFound($"Partner with ID {id} not found for deletion.");
                }

                return NoContent(); // Successful soft delete
            }
            catch (Exception ex)
            {
                // Log error and return 500 Internal Server Error
                _logger.LogError(ex, $"Error occurred while soft deleting partner with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        } 
        #endregion

        #endregion
    }
}