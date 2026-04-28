using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.UserBadgeDto
{
    public class UserBadgeDto
    {
        [Key]
        public Guid Id { get; set; }= Guid.NewGuid();

        public Guid ProfileId { get; set; }

        public Guid BadgeId { get; set; }

        public string? BadgeName { get; set; }

        public string? BadgeCategory { get; set; }

        public DateTime EarnedAt { get; set; }
    }
}