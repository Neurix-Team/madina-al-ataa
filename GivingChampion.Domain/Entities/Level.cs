using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    public class Level : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Range(1, int.MaxValue)]
        public int Number { get; set; }
        [Range(0, int.MaxValue)]
        public int MaxXp { get; set; }

        public ICollection<Profile> Profiles { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }
    }
}
