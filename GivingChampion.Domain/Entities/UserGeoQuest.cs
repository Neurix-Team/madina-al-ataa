using GivingChampion.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GivingChampion.Domain.Entities
{
    public class UserGeoQuest : BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        [Required]
        [ForeignKey(nameof(GeoQuest))]
        public Guid GeoQuestId { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        [Required]
        public GeoQuest GeoQuest { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime StartedAt { get; set; }
        public bool IsStarted { get; set; } // Add this flag to indicate if the GeoQuest has started
        public bool IsLocationVerified { get; set; }
        public bool IsCompleted { get; set; }
    }
}
