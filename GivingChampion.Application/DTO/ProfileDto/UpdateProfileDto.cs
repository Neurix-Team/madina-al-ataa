using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.ProfileDto
{
    public class UpdateProfileDto
    {
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double Rating { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Impact must be 0 or greater")]
        public int Impact { get; set; }

        public Guid AvatarId { get; set; }

        public Guid LevelId { get; set; }
    }
}