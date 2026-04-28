using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class UserBadge : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }

        public Guid BadgeId { get; set; }
        public Badge Badge { get; set; }

        public DateTime EarnedAt { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }
    }
}