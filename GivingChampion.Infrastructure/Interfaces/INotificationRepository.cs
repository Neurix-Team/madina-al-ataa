using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Interfaces
{
    public interface INotificationRepository
    {
        /// <summary>
        /// Creates a new notification
        /// </summary>
        Task CreateAsync(Notification notification);

        /// <summary>
        /// Gets all notifications for a specific user
        /// </summary>
        Task<List<Notification>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Gets unread notifications for a user
        /// </summary>
        Task<List<Notification>> GetUnreadByUserIdAsync(Guid userId);

        /// <summary>
        /// Marks a notification as read
        /// </summary>
        Task MarkAsReadAsync(Guid userId,Guid notificationId);

        /// <summary>
        /// Marks all notifications as read for a user
        /// </summary>
        Task MarkAllAsReadAsync(Guid userId);

        /// <summary>
        /// Soft deletes a notification
        /// </summary>
        Task SoftDeleteAsync(Guid notificationId);

        /// <summary>
        /// Gets notification by ID
        /// </summary>
        Task<Notification?> GetByIdAsync(Guid id);
    }
}