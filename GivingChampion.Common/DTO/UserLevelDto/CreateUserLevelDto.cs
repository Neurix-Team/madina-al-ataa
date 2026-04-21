using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.UserLevelDto
{
    public class CreateUserLevelDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "XP must be 0 or greater")]
        public int Xp { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "KP must be 0 or greater")]
        public int Kp { get; set; }

        [Required]
        public Guid ProfileId { get; set; }

        [Required]
        public Guid LevelId { get; set; }
    }
}