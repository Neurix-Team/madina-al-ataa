using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Domain.Entities
{
    public class Activity : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid EntityId { get; set; }

        [StringLength(500)]
        public string Description { get; set; }


        public ActivityAction Action { get; set; }
        public ActivityEntityType EntityType { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }


    }
}