using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO
{
    public class CreateUserGeoQuestDto
    {
        [StringLength(200)]
        public string? Title { get; set; }
        [Required]

        public Guid GeoQuestId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
    }
}