using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class Badge
    {
        [Key]
        public Guid Id { get; set; }

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
    }
}