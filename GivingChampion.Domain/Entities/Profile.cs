using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GivingChampion.Domain.Entities
{
    public class Profile : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

        [Range(0, int.MaxValue)]
        public int Impact { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Level")]
        public Guid LevelId { get; set; }
        public Level Level { get; set; }
        public ICollection<Badge> Badges { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}