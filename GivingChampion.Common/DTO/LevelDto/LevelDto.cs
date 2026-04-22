using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.LevelDto
{
    public class LevelDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public int Number { get; set; }

        public int MaxXp { get; set; }
    }
}