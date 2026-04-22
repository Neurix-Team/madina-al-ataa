using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.Location
{
    public class LocationDto
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public int RequiredLevel { get; set; }
        [Required]
        [StringLength(100)]
        public string Longitude { get; set; }
        [Required]
        [StringLength(100)]
        public string Latitude { get; set; } 
    }
}