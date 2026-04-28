using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Common.DTO.LevelDto
{
    public class LevelDto
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Number { get; set; }

        public int MaxXp { get; set; }
    }
}