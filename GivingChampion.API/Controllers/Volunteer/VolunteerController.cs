using GivingChampion.Application.Interfaces.Volunteer;
using GivingChampion.Common.DTO.VolunteerDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Volunteer
{
    /// <summary>
    /// Handles all volunteer API operations.
    /// Only Admin users are allowed to access these endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VolunteerController : ControllerBase
    {
        #region Fields

        private readonly IVolunteerService _volunteerService;
        private readonly ILogger<VolunteerController> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the VolunteerController class.
        /// </summary>
        /// <param name="volunteerService">Service used to handle volunteer business logic.</param>
        /// <param name="logger">Logger used to track warnings and errors.</param>
        public VolunteerController(
            IVolunteerService volunteerService,
            ILogger<VolunteerController> logger)
        {
            _volunteerService = volunteerService;
            _logger = logger;
        }

        #endregion

        #region Query Methods

        #region GetVolunteerById

        /// <summary>
        /// Gets a volunteer by unique identifier.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns>Volunteer data if found; otherwise 404 Not Found.</returns>
       [Authorize(Roles = "User")]

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                // Get volunteer from service layer as DTO.
                var volunteer = await _volunteerService.GetByIdAsync(id);

                // Return 404 if volunteer does not exist.
                if (volunteer == null)
                {
                    _logger.LogWarning("Volunteer with ID {VolunteerId} not found.", id);
                    return NotFound($"Volunteer with ID {id} not found.");
                }

                // Return volunteer data.
                return Ok(volunteer);
            }
            catch (Exception ex)
            {
                // Log unexpected errors for production tracking.
                _logger.LogError(ex, "Error occurred while fetching volunteer with ID {VolunteerId}.", id);

                // Return generic error message to avoid exposing internal details.
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        //#region GetAllVolunteers

        ///// <summary>
        ///// Gets all volunteers.
        ///// </summary>
        ///// <returns>List of volunteers as DTOs.</returns>
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    try
        //    {
        //        // Get all volunteers from service layer as DTOs.
        //        var volunteers = await _volunteerService.GetAllAsync();

        //        // Return volunteers list.
        //        return Ok(volunteers);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log unexpected errors for production tracking.
        //        _logger.LogError(ex, "Error occurred while fetching all volunteers.");

        //        // Return generic error message to avoid exposing internal details.
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //#endregion

        #endregion

        #region Command Methods

        #region CreateVolunteer

        ///// <summary>
        ///// Creates a new volunteer.
        ///// Admin only.
        ///// </summary>
        ///// <param name="dto">Volunteer creation data.</param>
        ///// <returns>The created volunteer with 201 Created response.</returns>
        //[Authorize(Roles = "User")]
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateVolunteerDto dto)
        //{
        //    try
        //    {
        //        // Create volunteer using service layer.
        //        var createdVolunteer = await _volunteerService.CreateAsync(dto);

        //        // Return 201 Created with route to the created volunteer.
        //        return CreatedAtAction(
        //            nameof(GetById),
        //            new { id = createdVolunteer.Id },
        //            createdVolunteer);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log unexpected errors for production tracking.
        //        _logger.LogError(ex, "Error occurred while creating the volunteer.");

        //        // Return generic error message to avoid exposing internal details.
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        #endregion

        #region UpdateVolunteer

        ///// <summary>
        ///// Updates an existing volunteer.
        ///// Admin only.
        ///// </summary>
        ///// <param name="id">Volunteer id.</param>
        ///// <param name="dto">Volunteer update data.</param>
        ///// <returns>Updated volunteer data if found; otherwise 404 Not Found.</returns>
        //[Authorize(Roles = "User")]
        //[HttpPut("{id:guid}")]
        //public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVolunteerDto dto)
        //{
        //    try
        //    {
        //        // Update volunteer using service layer.
        //        var updatedVolunteer = await _volunteerService.UpdateAsync(id, dto);

        //        // Return 404 if volunteer does not exist.
        //        if (updatedVolunteer == null)
        //        {
        //            _logger.LogWarning("Volunteer with ID {VolunteerId} not found for update.", id);
        //            return NotFound($"Volunteer with ID {id} not found.");
        //        }

        //        // Return updated volunteer data.
        //        return Ok(updatedVolunteer);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log unexpected errors for production tracking.
        //        _logger.LogError(ex, "Error occurred while updating volunteer with ID {VolunteerId}.", id);

        //        // Return generic error message to avoid exposing internal details.
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        #endregion
        #endregion

    }
}