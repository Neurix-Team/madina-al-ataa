using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.BadgeDto
{
    public class BadgeDto
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? Requirement { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }
    }
}