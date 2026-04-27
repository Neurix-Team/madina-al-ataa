using GivingChampion.Application.Interfaces.DonationOrderService;
using GivingChampion.Common.DTO.DonationOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DonationOrdersController : ControllerBase
    {
        private readonly IDonationOrderService _donationOrderService;

        public DonationOrdersController(IDonationOrderService donationOrderService)
        {
            _donationOrderService = donationOrderService;
        }

        #region Query Methods
        [Authorize(Roles = "Donor, Admin")]
        // GET: api/DonationOrders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DonationOrderDetailsDto>> GetDonationOrderById(Guid id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                var isAdmin = User.Claims.Any(c => c.Type == "Role" && c.Value == "Admin");
                var donationOrder = await _donationOrderService.GetByIdAsync(id, Guid.Parse(userId), isAdmin);

                if (donationOrder == null)
                {
                    return NotFound($"Donation order with ID {id} not found.");
                }

                return Ok(donationOrder);
            }
            catch (Exception ex)
            {
                // Log.Error(ex, "Error getting donation order by ID: {Id}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "Donor, Admin")]
        // GET: api/DonationOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonationOrderReadDto>>> GetAllDonationOrders()
        {
            try
            {
                var donationOrders = await _donationOrderService.GetAllAsync();
                return Ok(donationOrders);
            }
            catch (Exception ex)
            {
                // Log.Error(ex, "Error getting all donation orders");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        #endregion

        #region Command Methods
        [Authorize(Roles = "Donor")]
        // POST: api/DonationOrders
        [HttpPost]
        public async Task<ActionResult> CreateDonationOrder([FromBody] CreateDonationOrderDto donationOrderDto)
        {
            try
            {
                var userId = new Guid(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!);
                await _donationOrderService.CreateAsync(donationOrderDto, userId);
                return CreatedAtAction(nameof(GetDonationOrderById), new { id = donationOrderDto.DonationRequestId }, donationOrderDto);
            }
            catch (Exception ex)
            {
                // Log.Error(ex, "Error creating donation order");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "Donor")]
        // PUT: api/DonationOrders/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDonationOrder(Guid id, [FromBody] UpdateDonationOrderDTO donationOrderDto)
        {
            try
            {
                var userId = new Guid(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!);
                await _donationOrderService.UpdateAsync(id, donationOrderDto, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
        #endregion
    }
}