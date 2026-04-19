using GivingChampion.Common.DTO.Notification;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Interfaces
{
    public interface INotificationService
    {
        Task<Result<List<NotificationDto>>> GetMyNotificationsAsync(Guid userId, bool unreadOnly = false);
        Task<Result> MarkAsReadAsync(Guid userId, Guid notificationId);
        Task<Result> MarkAllAsReadAsync(Guid userId);

        // Internal method for other services to send notifications
        Task CreateNotificationAsync(Notification notification);
    }
}