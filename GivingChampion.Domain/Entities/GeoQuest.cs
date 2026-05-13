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
        public Guid LocationId { get; set; }
        public Location Location { get; set; }
        [Range(0, int.MaxValue)]
        public int XPReward { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int KPReward { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int ImpactReward { get; set; } = 0;
    }
}
