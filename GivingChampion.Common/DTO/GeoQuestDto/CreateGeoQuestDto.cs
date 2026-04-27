using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO
{
    public class CreateGeoQuestDto
    {

        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        public Guid LocationId { get; set; }

    }
}