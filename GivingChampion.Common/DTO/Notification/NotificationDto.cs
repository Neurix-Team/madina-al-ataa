using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.DTO.Notification
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
        public Guid? LinkedEntityId { get; set; }
        public string? LinkedEntityType { get; set; }
    }
}
