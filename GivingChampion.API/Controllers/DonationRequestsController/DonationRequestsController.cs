using GivingChampion.Application.Interfaces.DonationRequest;
using GivingChampion.Common.DTO.DonationRequest;
using GivingChampion.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationRequestsController : ControllerBase
    {
        #region Feild
        private readonly IDonationRequestService _donationRequestService;

        #endregion 

        #region Constructor
        // Constructor to inject the DonationRequestService
        public DonationRequestsController(IDonationRequestService donationRequestService)
        {
            _donationRequestService = donationRequestService;
        } 
        #endregion

        #region Get Methods By Id 
        [Authorize(Roles = "Donor")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadDonationRequestDto>> GetById(Guid id)
        {
            try
            {
                var donationRequest = await _donationRequestService.GetByIdAsync(id);
                return Ok(donationRequest);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);  // Return 404 if DonationRequest is not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");  // Return 500 for other errors
            }
        }
        #endregion 

        #region Get Methods All
        [Authorize(Roles = "Donor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListDonationRequestDto>>> GetAll()
        {
            try
            {
                var donationRequests = await _donationRequestService.GetAllAsync();
                return Ok(donationRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");  // Return 500 for errors
            }
        }

        
        #endregion

        #region Post 
        [Authorize(Roles = "Parent")]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateDonationRequestDto dto)
        {
            try
            {
                // Calling AddAsync to add the donation request and get the result
                var createdDonationRequest = await _donationRequestService.AddAsync(dto);

                // Returning CreatedAtAction with the Id of the created request
                return CreatedAtAction(nameof(GetById), new { id = createdDonationRequest.Id }, createdDonationRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");  // Return 500 for unexpected errors
            }
        }
        #endregion

        #region Put
        [Authorize(Roles = "Parent")]

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateDonationRequestDto dto)
        {
            try
            {
                await _donationRequestService.UpdateAsync(id, dto);
                return NoContent();  // 204 No Content for successful update
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);  // 404 if DonationRequest is not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");  // 500 for other errors
            }
        }

        #endregion
    }
}