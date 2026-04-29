using GivingChampion.API.Extensions;
using GivingChampion.Application.Interfaces.VolunteerHistoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.VolunteerHistories
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VolunteerHistoriesController : ControllerBase
    {
        #region Fields

        private readonly IVolunteerHistoryService _historyService;

        #endregion

        #region Constructor

        public VolunteerHistoriesController(IVolunteerHistoryService historyService)
        {
            _historyService = historyService;
        }

        #endregion

        #region Get My History

        [HttpGet("me")]
        public async Task<IActionResult> GetMyHistory()
        {
            if (!User.TryGetCurrentUserId(out var userId))
                return Unauthorized(new { message = "Invalid or missing user ID in token." });

            var result = await _historyService.GetUserHistory(userId);

            return Ok(result);
        }

        #endregion

        #region Get Request History

        [HttpGet("request/{requestId:guid}")]
        public async Task<IActionResult> GetRequestHistory(Guid requestId)
        {
            var result = await _historyService.GetRequestHistory(requestId);

            return Ok(result);
        }

        #endregion
    }
}