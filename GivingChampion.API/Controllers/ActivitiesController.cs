using GivingChampion.Application.Interfaces;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<ActivitiesController> _logger;

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
