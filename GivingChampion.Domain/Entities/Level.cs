using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    public class Level
    {
        [Key]
        public Guid Id { get; set; }
        [Range(1, int.MaxValue)]
        public int Number { get; set; }
        [Range(0, int.MaxValue)]
        public int MaxXp { get; set; }
    }
}
