using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.UserBadgeDto
{
    public class UpdateUserBadgeDto
    {
        [Required]
        public Guid BadgeId { get; set; }
    }
}