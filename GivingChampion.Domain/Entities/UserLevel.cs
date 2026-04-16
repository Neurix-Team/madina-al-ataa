using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class UserLevel
    {
        [Key]
        public Guid Id { get; set; }

        [Range(0, int.MaxValue)]
        public int Xp { get; set; }

        [Range(0, int.MaxValue)]
        public int Kp { get; set; }

        public Guid LevelId { get; set; }
        public Level Level { get; set; } 
    }
}