using GivingChampion.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class UserLevel : ISoftDeletable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Range(0, int.MaxValue)]
        public int Xp { get; set; }

        [Range(0, int.MaxValue)]
        public int Kp { get; set; }

        public Guid LevelId { get; set; }
        public Level Level { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; } 
    }
}