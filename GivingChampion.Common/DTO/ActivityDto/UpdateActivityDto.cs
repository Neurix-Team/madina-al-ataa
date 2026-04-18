using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.ActivityDto
{
    public class UpdateActivityDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}