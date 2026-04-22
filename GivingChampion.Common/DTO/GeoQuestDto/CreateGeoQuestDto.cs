using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO
{
    public class CreateGeoQuestDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public Guid LocationId { get; set; }

    }
}