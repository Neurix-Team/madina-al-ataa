using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.UserBadgeDto
{
    public class UpdateUserBadgeDto
    {
        [Required]
        public Guid BadgeId { get; set; }
    }
}