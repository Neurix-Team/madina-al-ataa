using GivingChampion.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GivingChampion.Domain.Entities
{
    public class GeoQuest : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [ForeignKey("Location")]
        public Guid LocationId { get; set; }
        public Location Location { get; set; }

    }
}
