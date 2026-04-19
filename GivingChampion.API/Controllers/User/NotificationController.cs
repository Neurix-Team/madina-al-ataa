using GivingChampion.Application.Interfaces;
using GivingChampion.Common.DTO.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/notification")]
    [Produces("application/json")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

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
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _notificationService.GetMyNotificationsAsync(userId, unreadOnly);
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
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _notificationService.MarkAsReadAsync(userId, dto.NotificationId);
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
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return result.Succeeded ? NoContent() : BadRequest(result);
        }
    }
}