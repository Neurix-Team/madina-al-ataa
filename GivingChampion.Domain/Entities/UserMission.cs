using GivingChampion.Common.Enums;
using GivingChampion.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    [PrimaryKey("Id")]
    public class UserMission : ISoftDeletable
    {
        public Guid Id { get; set; }
        public int Progress { get; set; }
        public MissionStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        [ForeignKey("Mission")]
        public Guid MissionId { get; set; }
        public Mission Mission { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
