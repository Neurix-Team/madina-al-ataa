using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO
{
    public class UpdateGeoQuestDto
    {
        [Key]
        public Guid Id { get; set; }
     
        [StringLength(200)]
        public string? Title { get; set; }
        [Required]
        public Guid LocationId { get; set; }


    }
}