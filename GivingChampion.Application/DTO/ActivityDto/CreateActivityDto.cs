using System;
using System.Collections.Generic;
using System.Text;
using GivingChampion.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace GivingChampion.Application.DTO.ActivityDto
{
    public class CreateActivityDto
    {
        public Guid EntityId { get; set; }
        public Guid UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public ActivityAction Action { get; set; }
        public ActivityEntityType EntityType { get; set; }
    }
}
