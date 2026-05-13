using GivingChampion.Application.Interfaces;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// Handles HTTP requests for Activities.
    /// </summary>
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<ActivitiesController> _logger;

        /// <summary>
        /// Performs the ActivitiesController operation.
        /// </summary>
        /// <param name="activityService">Provides the activityService value required by the operation.</param>
        /// <param name="logger">Provides the logger value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public ActivitiesController(
            IActivityService activityService,
            ILogger<ActivitiesController> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        //[HttpGet("me")]
        //[Authorize(Roles = "Volunteer,Admin")]
        //public async Task<IActionResult> GetMyHistory([FromQuery] PageParameters pageParameters)
        //{
        //    try
        //    {
        //        var result = await _activityService.GetUserActivities(pageParameters);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while fetching history for authenticated user.");

        //        return StatusCode(500, new
        //        {
        //            message = "Error while fetching user history",
        //            error = ex.Message
        //        });
        //    }
        //}

        [HttpGet("entity/{entityId:guid}")]
        [Authorize]
        /// <summary>
        /// Performs the GetEntityHistory operation.
        /// </summary>
        /// <param name="entityId">Provides the entityId value required by the operation.</param>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetEntityHistory(
            Guid entityId,
            [FromQuery] PageParameters pageParameters)
        {
            try
            {
                var result = await _activityService.GetEntityHistory(entityId, pageParameters);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching history for entity {EntityId}", entityId);

                return StatusCode(500, new
                {
                    message = "Error while fetching entity history",
                    error = ex.Message
                });
            }
        }
    }
}
