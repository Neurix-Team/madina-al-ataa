using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.UserBadgeDto
{
    public class CreateUserBadgeDto
    {
        [Required]
        public Guid ProfileId { get; set; }

        [Required]
        public Guid BadgeId { get; set; }
    }
}