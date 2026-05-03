using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.AiAvatarDto
{
    public class UpdateAiAvatarDto
    {
        [StringLength(100)]
        public string? FavoriteCategory { get; set; }

        [Range(0, 10, ErrorMessage = "Success rate must be between 0 and 10")]
        public int SuccessRate { get; set; }

        [Required]
        [StringLength(500)]
        public string? LastSuggestion { get; set; } 
    }
}