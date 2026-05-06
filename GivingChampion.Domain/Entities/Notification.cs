using GivingChampion.Common.Enums;
using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    [PrimaryKey("Id")]
    public class Notification : BaseEntity
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public Guid? LinkedEntityId { get; set; }
        public string LinkedEntityType { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }
    }
}
