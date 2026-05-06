using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.UserBadge
{
    public class CreateUserBadgeDto
    {
        [Required]
        public Guid ProfileId { get; set; }

        [Required]
        public Guid BadgeId { get; set; }
    }
}