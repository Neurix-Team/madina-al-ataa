using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.UserBadgeDto
{
    public class UserBadgeDto
    {
        public Guid Id { get; set; }

        public Guid ProfileId { get; set; }

        public Guid BadgeId { get; set; }

        public string? BadgeName { get; set; }

        public string? BadgeCategory { get; set; }

        public DateTime EarnedAt { get; set; }
    }
}