using GivingChampion.Common.Enums;
using GivingChampion.Common.Interfaces;
using GivingChampion.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    [PrimaryKey("Id")]
    public class Mission : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int RequiredLevel { get; set; }
        public int KPReward { get; set; }
        public int XPReward { get; set; }
        public int ImpactReward { get; set; }
        public MissionStatus Status { get; set; }
        [ForeignKey("Location")]
        public Guid LocationId { get; set; }
        public Location Location { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? DeletedAt { get; set; }
    }
}
