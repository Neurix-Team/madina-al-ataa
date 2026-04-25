using GivingChampion.Application.Interfaces.VolunteerHistoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GivingChampion.API.Controllers.VolunteerHistories
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VolunteerHistoriesController : ControllerBase
    {
        #region Fields

        private readonly IVolunteerHistoryService _historyService;
        private readonly ILogger<VolunteerHistoriesController> _logger;

        #endregion

        #region Constructor

        public VolunteerHistoriesController(
            IVolunteerHistoryService historyService,
            ILogger<VolunteerHistoriesController> logger)
        {
            _historyService = historyService;
            _logger = logger;
        }

        #endregion

        #region Get User History

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserHistory(Guid userId)
        {
            try
            {
                _logger.LogInformation("Fetching history for user {UserId}", userId);

                var result = await _historyService.GetUserHistory(userId);

                if (result == null || !result.Any())
                {
                    return NotFound("No history found for this user.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching history for user {UserId}", userId);

                return StatusCode(500, new
                {
                    message = "Error while fetching user history",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region Get Request History

        [HttpGet("request/{requestId:guid}")]
        public async Task<IActionResult> GetRequestHistory(Guid requestId)
        {
            try
            {
                var result = await _historyService.GetRequestHistory(requestId);

                if (result == null || !result.Any())
                {
                    return NotFound("No history found for this request.");
                }

                return Ok(result);

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

        #endregion
    }
}