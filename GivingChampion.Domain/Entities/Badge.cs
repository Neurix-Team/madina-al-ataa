using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GivingChampion.Domain.Entities
{
    public class Badge : BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } 

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(300)]
        public string? Requirement { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}