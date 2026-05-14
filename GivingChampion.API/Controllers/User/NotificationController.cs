using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO.Notification;
using GivingChampion.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/notification")]
    [Produces("application/json")]
    [Authorize]
    /// <summary>
    /// Handles HTTP requests for Notifications.
    /// </summary>
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Performs the NotificationsController operation.
        /// </summary>
        /// <param name="notificationService">Provides the notificationService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Gets all notifications for the current user
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(Result<List<NotificationDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<List<NotificationDto>>>> GetNotifications([FromQuery] bool unreadOnly = false)
        {
            var result = await _notificationService.GetMyNotificationsAsync(unreadOnly);
            return Ok(result);
        }

        /// <summary>
        /// Marks a specific notification as read
        /// </summary>
        [HttpPost("mark-read")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadDto dto)
        {
            var result = await _notificationService.MarkAsReadAsync(dto.NotificationId);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }

        /// <summary>
        /// Marks all notifications as read for the current user
        /// </summary>
        [HttpPost("mark-all-read")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var result = await _notificationService.MarkAllAsReadAsync();
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}
