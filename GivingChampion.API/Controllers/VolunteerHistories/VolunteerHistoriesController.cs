using GivingChampion.API.Extensions;
using GivingChampion.Application.Interfaces.VolunteerHistoryService;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.VolunteerHistories
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VolunteerHistoriesController : ControllerBase
    {
        private readonly IVolunteerHistoryService _historyService;

        public VolunteerHistoriesController(
            IVolunteerHistoryService historyService,
            ILogger<VolunteerHistoriesController> logger)
        {
            _historyService = historyService;
        }

        [HttpGet("me")]
        [Authorize(Roles = "Volunteer,Admin")]
        public async Task<IActionResult> GetMyHistory([FromQuery] PageParameters pageParameters)
        {
            if (!User.TryGetCurrentUserId(out var userId))
                return Unauthorized(new { message = "Invalid or missing user ID in token." });

                var result = await _historyService.GetUserHistory(userId, pageParameters);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching history for authenticated user.");

                return StatusCode(500, new
                {
                    message = "Error while fetching user history",
                    error = ex.Message
                });
            }
        }

        [HttpGet("request/{requestId:guid}")]
        [Authorize(Roles = "Volunteer,Admin")]
        public async Task<IActionResult> GetRequestHistory(
            Guid requestId,
            [FromQuery] PageParameters pageParameters)
        {
            try
            {
                var result = await _historyService.GetRequestHistory(requestId, pageParameters);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching history for request {RequestId}", requestId);

                return StatusCode(500, new
                {
                    message = "Error while fetching request history",
                    error = ex.Message
                });
            }
        }
    }
}