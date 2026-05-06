using GivingChampion.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.DTO.Notification
{
    public class CreateNotificationDto
    {
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid? LinkedEntityId { get; set; }
        public string? LinkedEntityType { get; set; }
        public Guid UserId { get; set; }
    }
}
