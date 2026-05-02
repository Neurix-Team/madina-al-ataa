using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.Location
{
    public class CreateLocationDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 100)]
        public int RequiredLevel { get; set; }

        [Required]
        public string Longitude { get; set; } = string.Empty;

        [Required]
        public string Latitude { get; set; } = string.Empty;
    }
}