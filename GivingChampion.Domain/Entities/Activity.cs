using GivingChampion.Domain.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class Activity : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } 


    }
}