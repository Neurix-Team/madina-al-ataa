using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.VolunteerOrder
{
    public class UpdateVolunteerOrderProgressDto
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Progress must be between 1 and 100")]
        public int Progress { get; set; }
    }
}