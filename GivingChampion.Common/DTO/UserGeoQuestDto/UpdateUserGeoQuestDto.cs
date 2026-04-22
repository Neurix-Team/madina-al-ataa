using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO
{
    public class UpdateUserGeoQuestDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        [Required]

        public Guid GeoQuestId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}