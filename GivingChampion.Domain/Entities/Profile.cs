using GivingChampion.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class Profile : ISoftDeletable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Range(0, 5)]
        public double Rating { get; set; }

        [Range(0, int.MaxValue)]
        public int Impact { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid AvatarId { get; set; }
        public Avatar Avatar { get; set; } 

        public Guid LevelId { get; set; }
        public Level Level { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Badge> Badges { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}