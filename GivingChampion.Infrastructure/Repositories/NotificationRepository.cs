using GivingChampion.Application.Interfaces;
using GivingChampion.Persistence.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Notification notification)
        {
            notification.SentAt = DateTime.UtcNow;
            notification.IsRead = false;
            await _context.Notifications.AddAsync(notification);
        }

        public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

        }

        public async Task SoftDeleteAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);
        }
    }
}
