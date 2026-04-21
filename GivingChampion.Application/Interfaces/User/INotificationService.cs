using GivingChampion.Common.DTO.Notification;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Interfaces
{
    public interface INotificationService
    {
        Task<Result<PagedList<NotificationDto>>> GetMyNotificationsAsync(Guid userId, bool unreadOnly = false, PageParameters pageParameters = null);
        Task<Result> MarkAsReadAsync(Guid userId, Guid notificationId);
        Task<Result> MarkAllAsReadAsync(Guid userId);

        // Internal method for other services to send notifications
        Task CreateNotificationAsync(Notification notification);
    }
}