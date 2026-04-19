using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class Profile
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

        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
        public ICollection<UserLevel> UserLevels { get; set; } = new List<UserLevel>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}