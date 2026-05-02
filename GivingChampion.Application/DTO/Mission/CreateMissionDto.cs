using GivingChampion.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.Mission
{
    public class CreateMissionDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DifficultyLevel Difficulty { get; set; }

        [Required]
        [Range(1, 100)]
        public int RequiredLevel { get; set; }

        [Required]
        [Range(1, 10000)]
        public int KPReward { get; set; }

        [Required]
        [Range(1, 10000)]
        public int XPReward { get; set; }

        [Required]
        [Range(1, 10000)]
        public int ImpactReward { get; set; }

        [Required]
        public Guid LocationId { get; set; }
    }
}