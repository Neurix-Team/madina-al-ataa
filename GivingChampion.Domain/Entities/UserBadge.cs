using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class UserBadge
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }

        public Guid BadgeId { get; set; }
        public Badge Badge { get; set; }

        public DateTime EarnedAt { get; set; }
    }
}