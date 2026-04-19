using GivingChampion.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class UserBadge : ISoftDeletable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }

        public Guid BadgeId { get; set; }
        public Badge Badge { get; set; }

        public DateTime EarnedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}